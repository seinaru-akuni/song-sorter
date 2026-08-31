using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory; // <--- ДОДАНО ДЛЯ КЕШУВАННЯ
using SongSorterWebAPI.Data;
using SongSorterWebAPI.Services;

namespace SongSorterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenProtectionService _tokenProtection;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache; // <--- ДОДАНО

        public PlaylistsController(
            AppDbContext context,
            ITokenProtectionService tokenProtection,
            IConfiguration configuration,
            IMemoryCache cache) // <--- ДОДАНО В ІНЖЕКЦІЮ
        {
            _context = context;
            _tokenProtection = tokenProtection;
            _configuration = configuration;
            _cache = cache; // <--- ДОДАНО
        }

        [HttpGet("my-playlists")]
        [Authorize]
        public async Task<IActionResult> GetPlaylists([FromQuery] string email)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int currentAppUserId))
            {
                return Unauthorized(new { message = "Не вдалося ідентифікувати користувача додатку." });
            }

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Не вказано email акаунта Google." });
            }

            // 1. Формуємо унікальний ключ для кешу (наприклад: "youtube_token_5_test@gmail.com")
            string cacheKey = $"youtube_token_{currentAppUserId}_{email}";


            // 2. Перевіряємо, чи є вже живий Access Token у кеші
            string? accessToken = await GetOrRefreshAccessTokenAsync(currentAppUserId, email, cacheKey);

            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest(new { message = "Акаунт не знайдено або він потребує повторної авторизації (немає Refresh Token)." });
            }

            var response = await CallYouTubeApiAsync(accessToken);


            // 3. Якщо токен недійсний (401) або бракує прав (403), робимо ПРИМУСОВЕ оновлення
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                _cache.Remove(cacheKey); // Очищаємо неробочий токен з кешу

                // Пробуємо отримати свіжий токен в обхід кешу
                accessToken = await GetOrRefreshAccessTokenAsync(currentAppUserId, email, cacheKey, forceRefresh: true);

                if (string.IsNullOrEmpty(accessToken))
                {
                    return StatusCode(500, new { message = "Не вдалося оновити ключ доступу Google." });
                }

                // 4. Повторюємо запит до YouTube з новим токеном
                response = await CallYouTubeApiAsync(accessToken);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorDetail = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"YouTube API Error: {errorDetail}");

                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return StatusCode(403, new { message = "Недостатньо прав для доступу до YouTube. Будь ласка, переавторизуйте акаунт Google і надайте дозволи.", details = errorDetail });
                }

                return StatusCode((int)response.StatusCode, new { message = "Помилка при зверненні до YouTube API", details = errorDetail });
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<object>(content);

            return Ok(json);
        }

        private async Task<string?> GetOrRefreshAccessTokenAsync(int userId, string email, string cacheKey, bool forceRefresh = false)
        {
            // Якщо ми не вимагаємо примусового оновлення, шукаємо в кеші
            if (!forceRefresh && _cache.TryGetValue(cacheKey, out string? cachedToken))
            {
                return cachedToken;
            }

            // Йдемо в базу за Refresh Token
            var linkedAccount = await _context.LinkedAccounts
                .FirstOrDefaultAsync(la => la.AppUserId == userId
                                        && la.ProviderName == "Google"
                                        && la.Email == email);

            if (linkedAccount == null || string.IsNullOrEmpty(linkedAccount.RefreshToken))
            {
                return null;
            }

            var refreshToken = _tokenProtection.DecryptToken(linkedAccount.RefreshToken);

            // Отримуємо новий токен від Google
            var newAccessToken = await RefreshGoogleAccessToken(refreshToken);

            if (!string.IsNullOrEmpty(newAccessToken))
            {
                // Зберігаємо отриманий токен у кеш на 50 хвилин
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(50));

                _cache.Set(cacheKey, newAccessToken, cacheOptions);
            }

            return newAccessToken;
        }

        private async Task<HttpResponseMessage> CallYouTubeApiAsync(string accessToken)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var youtubeApiUrl = "https://www.googleapis.com/youtube/v3/playlists?part=snippet,contentDetails&mine=true&maxResults=50";
            return await httpClient.GetAsync(youtubeApiUrl);
        }

        private async Task<string?> RefreshGoogleAccessToken(string refreshToken)
        {
            var values = new Dictionary<string, string>
            {
                { "client_id", _configuration["Authentication:Google:ClientId"]! },
                { "client_secret", _configuration["Authentication:Google:ClientSecret"]! },
                { "refresh_token", refreshToken },
                { "grant_type", "refresh_token" }
            };

            using var client = new HttpClient();
            var response = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(values));

            if (!response.IsSuccessStatusCode) return null;

            var responseString = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<JsonElement>(responseString);

            return tokenData.GetProperty("access_token").GetString();
        }
    }
}

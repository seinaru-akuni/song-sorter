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
            if (!_cache.TryGetValue(cacheKey, out string? accessToken))
            {
                // ЯКЩО В КЕШІ НЕМАЄ (або час вийшов) -> Йдемо в базу і до Google

                var linkedAccount = await _context.LinkedAccounts
                    .FirstOrDefaultAsync(la => la.AppUserId == currentAppUserId
                                            && la.ProviderName == "Google"
                                            && la.Email == email);

                if (linkedAccount == null || string.IsNullOrEmpty(linkedAccount.RefreshToken))
                {
                    return BadRequest(new { message = "Акаунт не знайдено або він потребує повторної авторизації." });
                }

                var refreshToken = _tokenProtection.DecryptToken(linkedAccount.RefreshToken);

                // Отримуємо новий токен від Google
                accessToken = await RefreshGoogleAccessToken(refreshToken);

                if (string.IsNullOrEmpty(accessToken))
                {
                    return StatusCode(500, new { message = "Не вдалося оновити ключ доступу." });
                }

                // 3. Зберігаємо отриманий токен у кеш на 50 хвилин
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(50));

                _cache.Set(cacheKey, accessToken, cacheOptions);
            }

            // 4. Робимо запит до YouTube (accessToken береться або з кешу, або щойно отриманий)
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var youtubeApiUrl = "https://www.googleapis.com/youtube/v3/playlists?part=snippet&mine=true&maxResults=50";
            var response = await httpClient.GetAsync(youtubeApiUrl);

            // Якщо токен якимось дивом виявився невалідним (401 Unauthorized), очищаємо кеш
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _cache.Remove(cacheKey);
                return StatusCode(401, new { message = "Токен YouTube недійсний. Спробуйте оновити сторінку." });
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorDetail = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorDetail);
                return StatusCode((int)response.StatusCode, "Помилка при зверненні до YouTube API");
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<object>(content);

            return Ok(json);
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
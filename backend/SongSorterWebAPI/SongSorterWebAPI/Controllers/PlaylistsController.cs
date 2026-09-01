using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
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
        
        private readonly IMemoryCache _cache; 
        private readonly IYtPlaylistsService _ytPlaylists; 
        private readonly IGoogleAccessTokenService _googleAccessToken;

        public PlaylistsController(IMemoryCache cache, IYtPlaylistsService ytPlaylists, IGoogleAccessTokenService googleAccessToken) 
        {
            _cache = cache;
            _ytPlaylists = ytPlaylists;
            _googleAccessToken = googleAccessToken;
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
            string? accessToken = await _googleAccessToken.GetOrRefreshAccessTokenAsync(currentAppUserId, email, cacheKey);

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
                accessToken = await _googleAccessToken.GetOrRefreshAccessTokenAsync(currentAppUserId, email, cacheKey, forceRefresh: true);

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

            // Парсимо JSON як вузол, щоб його можна було редагувати
            var jsonObject = JsonNode.Parse(content)?.AsObject();
            var itemsArray = jsonObject?["items"]?.AsArray();

            if (itemsArray != null)
            {
                // 1. Шукаємо ID плейлиста "Сподобалося"
                var likedPlaylistId = await _ytPlaylists.GetLikedVideosPlaylistIdAsync(accessToken);

                if (!string.IsNullOrEmpty(likedPlaylistId))
                {
                    // 2. Запитуємо деталі цього плейлиста
                    var likedPlaylistResponse = await _ytPlaylists.GetPlaylistByIdAsync(accessToken, likedPlaylistId);

                    if (likedPlaylistResponse.IsSuccessStatusCode)
                    {
                        var likedContent = await likedPlaylistResponse.Content.ReadAsStringAsync();
                        var likedJson = JsonNode.Parse(likedContent);
                        var likedItems = likedJson?["items"]?.AsArray();

                        // 3. Якщо отримали дані, додаємо плейлист "Сподобалося" на початок списку
                        if (likedItems != null && likedItems.Count > 0)
                        {
                            // Клонуємо об'єкт (перетворюємо в строку і назад), щоб безпечно вставити його в інший масив
                            var likedItemNode = JsonNode.Parse(likedItems[0]!.ToJsonString());
                            itemsArray.Insert(0, likedItemNode);
                        }
                    }
                }
            }

            // Повертаємо оновлений об'єкт, який тепер містить і "Сподобалося", і власні плейлисти
            return Ok(jsonObject);
        }

        

        private async Task<HttpResponseMessage> CallYouTubeApiAsync(string accessToken)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var youtubeApiUrl = "https://www.googleapis.com/youtube/v3/playlists?part=snippet,contentDetails&mine=true&maxResults=50";
            return await httpClient.GetAsync(youtubeApiUrl);
        }

        

        

        
    }
}

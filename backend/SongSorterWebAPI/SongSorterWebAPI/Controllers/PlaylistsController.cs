using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SongSorterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        [HttpGet("my-playlists")]
        public async Task<IActionResult> GetPlaylists()
        {
            // 1. Читаємо токен, який нам передасть React у заголовку
            var authHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Немає доступу. Токен відсутній.");
            }

            // Відрізаємо слово "Bearer ", щоб отримати чистий токен
            var accessToken = authHeader.Substring("Bearer ".Length).Trim();

            // 2. Створюємо запит до офіційного YouTube API
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Параметри: part=snippet (отримати назви та обкладинки), mine=true (тільки мої плейлісти)
            var youtubeApiUrl = "https://www.googleapis.com/youtube/v3/playlists?part=snippet&mine=true&maxResults=50";

            var response = await httpClient.GetAsync(youtubeApiUrl);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Помилка при зверненні до YouTube API");
            }

            // 3. Читаємо відповідь від Google і одразу передаємо її на фронтенд
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<object>(content);

            return Ok(json);
        }
    }
}

using System.Net.Http.Headers;
using System.Text.Json;

namespace SongSorterWebAPI.Services
{
    public class YtPlayListsService : IYtPlaylistsService 
    {
        public async Task<HttpResponseMessage> GetPlaylistByIdAsync(string accessToken, string playlistId)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Використовуємо такий самий part=snippet,contentDetails, щоб структура була ідентичною
            var youtubeApiUrl = $"https://www.googleapis.com/youtube/v3/playlists?part=snippet,contentDetails&id={playlistId}";
            return await httpClient.GetAsync(youtubeApiUrl);
        }

        public async Task<string?> GetLikedVideosPlaylistIdAsync(string accessToken)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Запитуємо інформацію про канал користувача
            var channelsApiUrl = "https://www.googleapis.com/youtube/v3/channels?part=contentDetails&mine=true";
            var response = await httpClient.GetAsync(channelsApiUrl);

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();

            // Парсимо JSON, щоб дістати ID плейлиста з лайками
            using var doc = JsonDocument.Parse(content);
            var items = doc.RootElement.GetProperty("items");

            if (items.GetArrayLength() > 0)
            {
                return items[0]
                    .GetProperty("contentDetails")
                    .GetProperty("relatedPlaylists")
                    .GetProperty("likes")
                    .GetString(); // Поверне щось на зразок "LL..." (Liked List)
            }

            return null;
        }
    }
}

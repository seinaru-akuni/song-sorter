using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SongSorterWebAPI.Data;
using System.Text.Json;

namespace SongSorterWebAPI.Services
{
    public class GoogleAccessTokenService : IGoogleAccessTokenService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly ITokenProtectionService _tokenProtection;

        public GoogleAccessTokenService(AppDbContext context, IConfiguration configuration, IMemoryCache cache, ITokenProtectionService tokenProtection)
        {
            _context = context;
            _configuration = configuration;
            _cache = cache;
            _tokenProtection = tokenProtection;
        }
        public async Task<string?> RefreshGoogleAccessToken(string refreshToken)
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

        public async Task<string?> GetOrRefreshAccessTokenAsync(int userId, string email, string cacheKey, bool forceRefresh = false)
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
    }
}

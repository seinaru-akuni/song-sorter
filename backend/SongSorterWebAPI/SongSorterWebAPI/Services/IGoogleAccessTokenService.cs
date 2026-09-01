namespace SongSorterWebAPI.Services
{
    public interface IGoogleAccessTokenService
    {
        Task<string?> RefreshGoogleAccessToken(string refreshToken);
        Task<string?> GetOrRefreshAccessTokenAsync(int userId, string email, string cacheKey, bool forceRefresh = false);
    }
}

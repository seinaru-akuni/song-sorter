namespace SongSorterWebAPI.Services
{
    public interface IYtPlaylistsService
    {
        Task<HttpResponseMessage> GetPlaylistByIdAsync(string accessToken, string playlistId);
        Task<string?> GetLikedVideosPlaylistIdAsync(string accessToken);

    }
}

namespace SongSorterWebAPI.Services
{
    public interface ITokenProtectionService
    {
        string EncryptToken(string token);
        string DecryptToken(string encryptedToken);
    }
}

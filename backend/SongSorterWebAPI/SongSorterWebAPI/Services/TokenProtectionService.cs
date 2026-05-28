using Microsoft.AspNetCore.DataProtection;

namespace SongSorterWebAPI.Services
{
    public class TokenProtectionService : ITokenProtectionService
    {
        private readonly IDataProtector _protector;

        public TokenProtectionService(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("SongSorter.GoogleTokens");
        }

        public string EncryptToken(string token) => _protector.Protect(token);

        public string DecryptToken(string encryptedToken) => _protector.Unprotect(encryptedToken);
    }
}


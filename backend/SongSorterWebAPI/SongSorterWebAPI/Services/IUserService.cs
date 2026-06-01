using SongSorterWebAPI.Data;
using SongSorterWebAPI.Models;

namespace SongSorterWebAPI.Services
{
    public interface IUserService
    {
        public Task<bool> IsEmailTakenAsync(string email);

        public Task<bool> IsUsernameTakenAsync(string username);
        public Task<int> ContextSaveChangesAsync();
        public void AddNewAppUser(AppUser newUser);
        public Task<AppUser?> FindUserViaEmailAsync(string email);
        public Task<AppUser?> FindUserViaIdAsync(int id);
    }
}

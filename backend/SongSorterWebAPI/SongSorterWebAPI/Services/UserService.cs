using Microsoft.EntityFrameworkCore;
using SongSorterWebAPI.Data;
using SongSorterWebAPI.Models;
using System.Security.Cryptography;

namespace SongSorterWebAPI.Services
{
    public class UserService : IUserService
    {
        readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }        

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            if(await _context.AppUsers.AnyAsync(u => u.Email == email))
                return true;
            else return false;
        }

        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            if (await _context.AppUsers.AnyAsync(u => u.Username == username))
                return true;
            else return false;
        }

        public void AddNewAppUser(AppUser newUser)
        {
            _context.AppUsers.Add(newUser);
        }

        public async Task<int> ContextSaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<AppUser?> FindUserViaEmailAsync(string email)
        {
            return _context.AppUsers.FirstOrDefault(u => u.Email == email);
        }

        public async Task<AppUser?> FindUserViaIdAsync(int id)
        {
            return _context.AppUsers.FirstOrDefault(u => u.Id == id);
        }
    }
}

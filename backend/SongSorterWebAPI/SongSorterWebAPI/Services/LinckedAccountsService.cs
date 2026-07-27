using SongSorterWebAPI.Data;
using SongSorterWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace SongSorterWebAPI.Services
{
    public class LinckedAccountsService : ILinkedAccountService
    {
        readonly AppDbContext _context;

        public LinckedAccountsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LinkedAccount>> GetUserLinkedAccountsListAsync(int userId)
        {
            return await _context.LinkedAccounts
                .Where(la => la.AppUserId == userId) // Відфільтровуємо за ID юзера
                .ToListAsync();                    // Асинхронно перетворюємо результат на List
        }
    }
}
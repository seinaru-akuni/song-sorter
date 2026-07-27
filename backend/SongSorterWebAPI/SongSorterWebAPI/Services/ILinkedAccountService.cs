using Microsoft.EntityFrameworkCore;
using SongSorterWebAPI.Models;

namespace SongSorterWebAPI.Services
{
    public interface ILinkedAccountService
    {
        Task<List<LinkedAccount>> GetUserLinkedAccountsListAsync(int userId);
    }
}

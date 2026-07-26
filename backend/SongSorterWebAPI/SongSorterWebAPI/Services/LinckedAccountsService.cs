using SongSorterWebAPI.Data;

namespace SongSorterWebAPI.Services
{
    public class LinckedAccountsService : ILinkedAccountService
    {
        readonly AppDbContext _context;

        public LinckedAccountsService(AppDbContext context)
        {
            _context = context;
        }


    }
}

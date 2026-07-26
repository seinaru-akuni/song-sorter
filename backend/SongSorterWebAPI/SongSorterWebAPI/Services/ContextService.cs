using Microsoft.EntityFrameworkCore;
using SongSorterWebAPI.Data;

namespace SongSorterWebAPI.Services
{
    public class ContextService : IContextService
    {
        readonly AppDbContext _context;

        public ContextService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> ContextSaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

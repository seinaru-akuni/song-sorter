using Microsoft.EntityFrameworkCore;
using SongSorterWebAPI.Models;

namespace SongSorterWebAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<LinkedAccount> LinkedAccounts { get; set; }  
    }
}

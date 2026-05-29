using Microsoft.EntityFrameworkCore;
using SongSorterWebAPI.Models;

namespace SongSorterWebAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Ця властивість перетвориться на таблицю "Users" у SQL Server
        public DbSet<LinkedAccount> GoogleUsers { get; set; }  
    }
}

using GlobalGames.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GlobalGames.Data
{
   
        public class DataContext : DbContext
        {
            public DbSet<User> Users { get; set; }

            public DbSet<Subscriber> Subscribers { get; set; }

            
            public DbSet<Lead> Leads { get; set; }

            public DataContext(DbContextOptions<DataContext> options) : base(options)
            {
            }
        }
}

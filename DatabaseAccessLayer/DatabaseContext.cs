using DatabaseAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DatabaseAccessLayer
{
    public class DatabaseContext : DbContext
    {
        private readonly IConfiguration config;

        public DbSet<UserModel> Users { get; private set; }
        public DbSet<PointsModel> Points { get; private set; }
        public DbSet<PointsLogModel> PointsLog { get; private set; }

        public DatabaseContext(IConfiguration config) 
        {
            this.config = config;

            // TODO: Remove in production
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            var connectionString = config.GetConnectionString("mysql");
            optionsBuilder.UseMySql(connectionString);
#else
            var connectionString = config.GetConnectionString("postgres");
            optionsBuilder.UseNpgsql(connectionString);
#endif
            base.OnConfiguring(optionsBuilder);
        }
    }
}

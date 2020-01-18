using DatabaseAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

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
            var connectionString = config.GetConnectionString("postgres");

            optionsBuilder.UseNpgsql(connectionString);

            base.OnConfiguring(optionsBuilder);
        }
    }
}

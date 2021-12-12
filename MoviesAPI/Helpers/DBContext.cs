using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoviesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Helpers
{
    public class DBContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DBContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            options.UseMySql(Configuration["ConnectionStrings:Database"], MySqlServerVersion.LatestSupportedServerVersion);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Director>().ToTable("directors", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Toplists>(eb => {
            eb.HasNoKey();
            eb.Property(v => v.user_id).HasColumnName("user_id");
            });
        }

        public DbSet<Director> directors { get; set; }
        public DbSet<Movie> movies { get; set; }
        public DbSet<Person> people { get; set; }
        public DbSet<Ratings> ratings { get; set; }
        public DbSet<Star> stars { get; set; }
        public DbSet<User> users { get; set; }

        public DbSet<Toplists> toplists { get; set; }
    }
}

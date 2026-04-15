using Microsoft.EntityFrameworkCore;
using IdeasCreativas.Models;

namespace IdeasCreativas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Idea> Ideas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .HasIndex(t => t.Name)
                .IsUnique();
        }
    }
}

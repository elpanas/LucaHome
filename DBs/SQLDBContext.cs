using LucaHome.Models;
using Microsoft.EntityFrameworkCore;

namespace LucaHome.DBs
{
    public class SQLDBContext : DbContext
    {
        public SQLDBContext(DbContextOptions<SQLDBContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(table => { 
                table.HasKey(u => u.Id);
                table.Property(u => u.Username)
                        .HasMaxLength(50)
                        .IsRequired();  
                table.Property(u => u.Password)
                        .HasMaxLength(255)
                        .IsRequired();
            });

            modelBuilder.Entity<Skill>(table => table.HasKey(s => s.Id));

            modelBuilder.Entity<Comment>(table => table.HasKey(c => c.Id));
        }

    }
}

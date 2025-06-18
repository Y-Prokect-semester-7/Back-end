using Microsoft.EntityFrameworkCore;
using static UserManagement.UserRequest;

namespace UserManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        //ef core
        public DbSet<User> Users { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                modelBuilder.Entity<User>().ToTable("users");

                modelBuilder.Entity<User>().HasKey(u => u.Id);

                modelBuilder.Entity<User>().Property(u => u.Id).HasColumnName("id");
                modelBuilder.Entity<User>().Property(u => u.Auth0Id).HasColumnName("auth0_id");
                modelBuilder.Entity<User>().Property(u => u.Username).HasColumnName("username");
                modelBuilder.Entity<User>().Property(u => u.Bio).HasColumnName("bio");
                modelBuilder.Entity<User>().Property(u => u.Email).HasColumnName("email");
                modelBuilder.Entity<User>().Property(u => u.CreatedAt).HasColumnName("created_at");
                modelBuilder.Entity<User>().Property(u => u.UpdatedAt).HasColumnName("updated_at");
            });
        }
    }

}

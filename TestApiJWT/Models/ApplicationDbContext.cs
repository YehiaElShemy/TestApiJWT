using Microsoft.EntityFrameworkCore;

namespace TestApiJWT.Helper
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Users.AddRange(new List<User>
        {
            new User { Id = 1, Name = "John Doe", Email = "john@example.com" },
            new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
        });
            context.SaveChanges();
        }
    }
}

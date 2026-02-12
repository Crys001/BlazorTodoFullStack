using Microsoft.EntityFrameworkCore;

namespace WebApiGemini.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<WebApiGemini.Models.TodoItem> TodoItems { get; set; }
        public DbSet<WebApiGemini.Models.User> Users { get; set; }
        public DbSet<WebApiGemini.Models.Category> Categories { get; set; }
    }
}

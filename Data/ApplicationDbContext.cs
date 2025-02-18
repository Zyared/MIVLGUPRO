using Microsoft.EntityFrameworkCore;
using MIVLGUPRO.Model;

namespace MIVLGUPRO.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> users { get; set; }
        public DbSet<Teacher> teachers { get; set; }
        public DbSet<Group> groups { get; set; }
        public DbSet<Practice> practices { get; set; }
        public DbSet<Tasks> tasks { get; set; }
        public DbSet<CorrectAnswer> correctanswers { get; set; }
        public DbSet<UserAnswers> useranswers { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Host=localhost;Port=5432;Database=MiVLGUPro;Username=postgres;Password=123;";
            optionsBuilder.UseNpgsql(connectionString);
        }

    }
}

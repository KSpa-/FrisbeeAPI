using Microsoft.EntityFrameworkCore;

namespace FrisbeeAPI
{
    public class AppDbContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    }

    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}

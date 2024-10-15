using Microsoft.EntityFrameworkCore;

namespace Imsat.Bashmaky.Web.Db
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}

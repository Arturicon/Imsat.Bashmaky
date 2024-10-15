using Imsat.Bashmaky.Model.Database;
using Imsat.Web.Toolkits.DatabaseLogger;
using Imsat.Web.Toolkits.DatabaseLogging;
using Microsoft.EntityFrameworkCore;

namespace Imsat.Bashmaky.Web
{
    public class LoggingDbContext : DbContext
    {
        public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options)
        {
        }

        public DbSet<LogModel> Logs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("logging");
        }
    }
}

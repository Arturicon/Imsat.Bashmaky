using Imsat.Bashmaky.Model.Database.Entities;
using Imsat.Bashmaky.Model.Database.Entities.Devices;
using Imsat.Bashmaky.Model.Database.Types;
using Microsoft.EntityFrameworkCore;


namespace Imsat.Bashmaky.Web
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Railway> Railways { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Terminal> Terminals { get; set; }
        public DbSet<Box> Boxes { get; set; }
        public DbSet<Bashmak> Bashmaks { get; set; }
        public DbSet<SignalTerminal> TerminalSignals { get; set; }
        public DbSet<SignalBox> BoxSignals { get; set; }
        public DbSet<SignalBashmak> BashmakSignals { get; set; }
        public DbSet<TrainAttachment> TrainAttachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BaseEntity>().UseTpcMappingStrategy();
            modelBuilder.Entity<BaseEntity>().Ignore(x => x.DS);
            modelBuilder.Entity<Bashmak>().Ignore(x => x.RailwayStr);
            modelBuilder.Entity<Bashmak>().HasIndex(x => x.Mac).IsUnique();
            modelBuilder.Entity<Box>().HasIndex(x => x.Mac).IsUnique();
            modelBuilder.Entity<Box>().Ignore(x => x.MaxBashmaks);
            modelBuilder.Entity<Box>().Ignore(x => x.PercentageOfFilling);

            modelBuilder.HasDefaultSchema("model");
        }
    }
}

using larp_server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    public class GamesContext : DbContext
    {
        public DbSet<Coord> Coords { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Room> Rooms { get; set; }

        public GamesContext(DbContextOptions<GamesContext> options) : base(options)
        {
            Database.Migrate();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .HasMany(c => c.CoordsList)
                .WithOne(p => p.Player);
            modelBuilder.Entity<Room>()
                .HasMany(c => c.CoordsList)
                .WithOne(r => r.Room);
            modelBuilder.Entity<Coord>().ToTable("Coords").HasKey(k => new { k.PlayerName, k.RoomName });
        }
    }

}

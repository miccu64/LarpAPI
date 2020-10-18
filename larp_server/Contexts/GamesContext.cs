using larp_server.Models;
using Microsoft.EntityFrameworkCore;

//using MySql.Data.Entity;
using MySql.Data.EntityFrameworkCore;

namespace Server.Models
{
    public class GamesContext : DbContext
    {
        public DbSet<Coord> Coords { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Room> Rooms { get; set; }

        public GamesContext(DbContextOptions<GamesContext> options) : base(options)
        {
            this.Database.Migrate();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>()
                .HasMany(c => c.CoordsList)
                .WithOne(r => r.Room);
            modelBuilder.Entity<Player>()
                .HasMany(c => c.CoordsList)
                .WithOne(p => p.Player);
            modelBuilder.Entity<Coord>().ToTable("Coords").HasKey(k => new { k.PlayerId, k.RoomId });
            
            //modelBuilder.Entity<Player>().ToTable("Player");
            //modelBuilder.Entity<Room>().ToTable("Room");
        }
    }

}

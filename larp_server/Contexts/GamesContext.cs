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
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerRoom>()
            .HasKey(t => new { t.PlayerName, t.RoomName });

            modelBuilder.Entity<PlayerRoom>()
                .HasOne(pt => pt.Player)
                .WithMany(p => p.PlayerRoomList)
                .HasForeignKey(pt => pt.PlayerName);

            modelBuilder.Entity<PlayerRoom>()
                .HasOne(pt => pt.Room)
                .WithMany(t => t.PlayerRoomList)
                .HasForeignKey(pt => pt.RoomName);

            modelBuilder.Entity<Coord>().ToTable("Coords");
            modelBuilder.Entity<Player>().ToTable("Player");
            modelBuilder.Entity<Room>().ToTable("Room");
        }
    }

}

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
            modelBuilder.Entity<Coord>().ToTable("Coords");
            modelBuilder.Entity<Player>().ToTable("Player");
            modelBuilder.Entity<Room>().ToTable("Room");
        }
    }

}

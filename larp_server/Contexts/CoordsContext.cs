using larp_server.Models;
using Microsoft.EntityFrameworkCore;

//using MySql.Data.Entity;
using MySql.Data.EntityFrameworkCore;

namespace Server.Models
{
    public class CoordsContext : DbContext
    {
        public DbSet<Coords> Coords { get; set; }

        public CoordsContext(DbContextOptions<CoordsContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coords>().ToTable("Coords");
        }
    }

}

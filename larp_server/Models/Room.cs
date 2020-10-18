using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace larp_server.Models
{
    public class Room
    {
        [Key]
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(30)]
        public string Password { get; set; }
        [StringLength(30)]
        public string Admin { get; set; }
        public DateTime LastPlayed { get; set; }
        public ICollection<Coord> CoordsList { get; set; }

        //EFCore needs empty constructors, even empty
        private Room() { }
        //my constructor
        public Room(string roomName, string password, Player player)
        {
            Name = roomName;
            Password = password;
            Admin = player.Name;
            LastPlayed = DateTime.UtcNow;
        }
    }
}

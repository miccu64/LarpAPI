using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace larp_server.Models
{
    public class Coord
    {
        [StringLength(30)]
        public string RoomId { get; set; }
        [StringLength(30)]
        public string PlayerId { get; set; }
        public Room Room { get; set; }
        public Player Player { get; set; }

        public int TeamId { get; set; }
        [StringLength(30)]
        public string ConnectionID { get; set; }
        public bool IsConnected { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        private Coord() { }
        public Coord(Room room, Player player, string conn)
        {
            Room = room;
            Player = player;
            PlayerId = player.Name;
            RoomId = room.Name;
            ConnectionID = conn;
            IsConnected = true;
            TeamId = 0;
            Latitude = 0;
            Longitude = 0;
        }
    }
}

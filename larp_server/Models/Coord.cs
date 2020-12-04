using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace larp_server.Models
{
    public class Coord
    {
        private readonly ILazyLoader lazyLoader;

        private Room room;
        private Player player;

        [StringLength(30)]
        public string RoomName { get; set; }
        [StringLength(30)]
        public string PlayerName { get; set; }
        public int TeamId { get; set; }
        public bool IsConnected { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        private Coord() { }
        //needed for collections to be not null
        private Coord(ILazyLoader lazyLoader) { this.lazyLoader = lazyLoader; }
        public Coord(Room room1, Player player1, int team)
        {
            room = room1;
            player = player1;
            RoomName = room.Name;
            PlayerName = player.Nickname;
            IsConnected = true;
            TeamId = team;
            Latitude = 0;
            Longitude = 0;
        }
        public Room Room
        {
            get => lazyLoader.Load(this, ref room);
            set => Room = value;
        }
        public Player Player
        {
            get => lazyLoader.Load(this, ref player);
            set => Player = value;
        }
    }
}

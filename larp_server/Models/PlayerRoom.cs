using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace larp_server.Models
{
    public class PlayerRoom
    {
        public string PlayerName { get; set; }
        public Player Player { get; set; }
        public string RoomName { get; set; }
        public Room Room { get; set; }
    }
}

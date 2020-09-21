using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace larp_server.Models
{
    public class Player
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<Room> RoomsList { get; set; }
    }
}

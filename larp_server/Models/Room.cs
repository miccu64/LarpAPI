using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace larp_server.Models
{
    public class Room
    {
        public string Name { get; set; }
        public ICollection<Player> PlayersList { get; set; }

    }
}

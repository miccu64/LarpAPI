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
        [StringLength(50)]
        public string Name { get; set; }
        public ICollection<PlayerRoom> PlayerRoomList { get; set; }

    }
}

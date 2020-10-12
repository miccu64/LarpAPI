using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace larp_server.Models
{
    public class Player
    {
        [Key]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        [StringLength(50)]
        public string Password { get; set; }
        public ICollection<PlayerRoom> PlayerRoomList { get; set; }
    }
}

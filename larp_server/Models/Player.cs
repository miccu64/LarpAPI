using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace larp_server.Models
{
    public class Player
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [StringLength(150)]
        public string Token { get; set; }

        [StringLength(80)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string Email { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(30)]
        public string Password { get; set; }

        public ICollection<Coord> CoordsList { get; set; }


        //EFCore needs empty constructors, even empty
        private Player() { }
        //my constructor
        public Player(string email, string name, string password, string token)
        {
            Email = email;
            Name = name;
            Password = password;
            Token = token;
        }
    }
}

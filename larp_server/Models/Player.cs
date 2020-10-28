using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace larp_server.Models
{
    public class Player
    {
        private readonly ILazyLoader lazyLoader;

        [Key]
        [StringLength(30)]
        public string Nickname { get; set; }
        [StringLength(150)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Token { get; set; }
        [StringLength(80)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Email { get; set; }
        [StringLength(30)]
        public string Password { get; set; }
        [StringLength(30)]
        public string ConnectionID { get; set; }
        [StringLength(30)]

        private ICollection<Coord> coordsList;


        //EFCore needs empty constructors, even empty
        private Player() { }
        //needed for collection CoordsList to be not null
        private Player(ILazyLoader lazyLoader) { this.lazyLoader = lazyLoader; }
        //my constructor
        public Player(string email, string name, string password, string token)
        {
            Email = email;
            Nickname = name;
            Password = password;
            Token = token;
            CoordsList = new Collection<Coord>();
        }
        //getter and setter for coords
        public ICollection<Coord> CoordsList
        {
            get => lazyLoader.Load(this, ref coordsList);
            set => coordsList = value;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace larp_server.Models
{
    public class Room
    {
        private readonly ILazyLoader lazyLoader;

        [Key]
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(30)]
        public string Password { get; set; }
        private Player admin;
        public DateTime LastPlayed { get; set; }
        private ICollection<Coord> coordsList;

        //EFCore needs empty constructors, even empty
        private Room() { }
        //needed for collection CoordsList to be not null
        private Room(ILazyLoader lazyLoader) { this.lazyLoader = lazyLoader; }
        //my constructor
        public Room(string roomName, string password, Player player)
        {
            Name = roomName;
            Password = password;
            Admin = player;
            LastPlayed = DateTime.UtcNow;
            CoordsList = new Collection<Coord>();
        }
        //getter and setter for coords
        public ICollection<Coord> CoordsList
        {
            get => lazyLoader.Load(this, ref coordsList);
            set => coordsList = value;
        }
        public Player Admin {
            get => lazyLoader.Load(this, ref admin);
            set => admin = value;
        }
    }
}

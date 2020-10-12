using larp_server.Models;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace larp_server
{
    public static class DbInitializer
    {
        public static void Initialize(GamesContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Coords.Any())
            {
                return;   // DB has been seeded
            }

            var coord = new Coord[]
            {
            new Coord{Id="aaa",Longitude=20,Latitude=30},
            new Coord{Id="asaa",Longitude=20,Latitude=30},
            new Coord{Id="adaa",Longitude=20,Latitude=30}
            };
            foreach (Coord s in coord)
            {
                context.Coords.Add(s);
            }
            context.SaveChanges();

            var player = new Player[]
            {
                new Player{Name="sss",Email="dsadsa"},
                new Player{Name="ssss",Email="dsasdsa"}
            };
            foreach (Player p in player)
            {
                context.Players.Add(p);
            }
            context.SaveChanges();

            var room = new Room[]
            {
                new Room{Name="dsad"}
            };
            context.Rooms.Add(room[0]);
            context.SaveChanges();
        }
    }
}

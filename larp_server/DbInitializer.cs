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
        public static void Initialize(CoordsContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Coords.Any())
            {
                return;   // DB has been seeded
            }

            var coord = new Coords[]
            {
            new Coords{Id="aaa",Longitude=20,Latitude=30},
            new Coords{Id="asaa",Longitude=20,Latitude=30},
            new Coords{Id="adaa",Longitude=20,Latitude=30}
            };
            foreach (Coords s in coord)
            {
                context.Coords.Add(s);
            }
            context.SaveChanges();
        }
    }
}

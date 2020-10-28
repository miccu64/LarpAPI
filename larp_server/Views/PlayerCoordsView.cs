using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace larp_server.Views
{
    public class PlayerCoordsView
    {
        public string PlayerName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public PlayerCoordsView(string name, double lat, double lon)
        {
            PlayerName = name;
            Latitude = lat;
            Longitude = lon;
        }
    }
}

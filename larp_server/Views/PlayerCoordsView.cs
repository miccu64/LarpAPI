using larp_server.Models;

namespace larp_server.Views
{
    public class PlayerCoordsView
    {
        public string PlayerName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public PlayerCoordsView(Coord coord)
        {
            PlayerName = coord.RoomName;
            Latitude = coord.Latitude;
            Longitude = coord.Longitude;
        }
    }
}

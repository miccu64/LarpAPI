using larp_server.Models;
using larp_server.Security;
using larp_server.Views;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace larp_server.Hubs
{
    //[EnableCors("MyPolicy")]
    public class GameHub : Hub
    {
        private readonly GamesContext db;
        private readonly JWTWorker JWTInstance;
        //needed because hub is short living and doesn't allow to use Clients when not trigerred
        private IHubContext<GameHub> hubContext = null;

        private TimeSpan startTimeSpan = TimeSpan.Zero;
        private TimeSpan periodTimeSpan = TimeSpan.FromSeconds(1);
        private Timer timer;
        public GameHub(GamesContext context, IHubContext<GameHub> context2)
        {
            db = context;
            hubContext = context2;
            JWTInstance = new JWTWorker();

            timer = new Timer((e) =>
            {


            }, null, startTimeSpan, periodTimeSpan);
        }
        ~GameHub()
        {
            timer = null;
            startTimeSpan = TimeSpan.Zero;
            periodTimeSpan = TimeSpan.FromSeconds(1);
        }
        public async Task SendMessage([Required] string message, [Required] bool toAll, [Required] string token)
        {
            if (!db.Players.Any(i => i.Token == token))
            {
                await Clients.Caller.SendAsync("GoToLogin", "Niepoprawny token. Zaloguj się ponownie.");
                return;
            }
            Player player = db.Players.First(p => p.Token == token);
            List<string> userIds = new List<string>();
            foreach (Coord c in player.CoordsList)
            {
                if (c.IsConnected == true)
                {
                    int team = c.TeamId;
                    foreach (Coord c2 in c.Room.CoordsList)
                    {
                        if ((c2.IsConnected == true) && (c2.TeamId == team || toAll == true))
                        {
                            userIds.Add(c2.Player.ConnectionID);
                        }
                    }
                    break;
                }
            }
            await Clients.Clients(userIds).SendAsync("GetChatMessage", message);
            //await Clients.Caller.SendAsync("GetChatMessage", message);
        }
        public async Task RegisterNewUser([Required] string email, [Required] string name, [Required] string password)
        {
            if (db.Players.Any(from => from.Email == email))
            {
                await Clients.Caller.SendAsync("LoginRegisterError", "Taki e-mail już istnieje. Podaj inny.");
                return;
            }
            if (db.Players.Any(from => from.Nickname == name))
            {
                await Clients.Caller.SendAsync("LoginRegisterError", "Taki login już istnieje. Podaj inny.");
                return;
            }
            string token = JWTInstance.Encode(name, email);
            Player player = new Player(email, name, password, token);
            await db.AddAsync(player);
            await db.SaveChangesAsync();
            await Clients.Caller.SendAsync("RegisterSuccess", "Pomyślnie zarejestrowano");
        }
        public async Task Login([Required] string email, [Required] string password)
        {
            if (db.Players.Any(p => p.Email == email && p.Password == password))
            {
                Player player = db.Players.First(p => p.Email == email);
                //send to player list of joined rooms
                List<string> list = new List<string>();
                foreach (Coord c in player.CoordsList)
                {
                    list.Add(c.RoomName);
                }
                string json = JsonSerializer.Serialize(list);
                await Clients.Caller.SendAsync("LoginSuccess", player.Token, json);
            }
            else await Clients.Caller.SendAsync("LoginRegisterError", "Niepoprawny login lub hasło.");
        }
        public async Task CreateRoom([Required] string roomName, [Required] string password, [Required] int team, [Required] string token)
        {
            if (!db.Players.Any(i => i.Token == token))
            {
                await Clients.Caller.SendAsync("GoToLogin", "Niepoprawny token. Zaloguj się ponownie.");
                return;
            }
            //check if room with that name exists
            if (db.Rooms.Any(from => from.Name == roomName))
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Taki pokój już istnieje. Podaj inną nazwę.");
                return;
            } 

            Player player = db.Players.First(i => i.Token == token);
            Room room = new Room(roomName, password, player);
            await db.Rooms.AddAsync(room);
            await db.SaveChangesAsync();
            //join to room
            await JoinRoom(roomName, password, team, token);
        }
        private async void AddCoord(Room room, Player player, int team)
        {
            Coord coord = new Coord(room, player, team);
            player.CoordsList.Add(coord);
            room.CoordsList.Add(coord);
            await db.Coords.AddAsync(coord);
        }
        public async Task JoinRoom([Required] string roomName, [Required] string password, [Required] int team, [Required] string token)
        {
            if (!db.Players.Any(i => i.Token == token))
            {
                await Clients.Caller.SendAsync("GoToLogin", "Niepoprawny token. Zaloguj się ponownie.");
                return;
            }
            //check if room with that name exists
            if (!db.Rooms.Any(from => from.Name == roomName))
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Taki pokój nie istnieje.");
                return;
            } 
            //check passwords
            Room room = db.Rooms.First(from => from.Name == roomName);
            if (room.Password != password)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Niepoprawne hasło.");
                return;
            }

            Player player = db.Players.First(from => from.Token == token);
            //disconnect player from all games
            foreach (Coord c in player.CoordsList)
            {
                c.IsConnected = false;
            }

            //check if player were playing that game
            if (!player.CoordsList.Any(c => c.RoomName == roomName))
                AddCoord(room, player, team);
            else
            {
                Coord coord = player.CoordsList.First(c => c.RoomName == roomName);
                coord.IsConnected = true;
            }
            player.ConnectionID = Context.ConnectionId;
            await db.SaveChangesAsync();
            await Clients.Caller.SendAsync("JoinedRoom", roomName);
        }
        public async Task JoinJoinedRoom([Required] string roomName, [Required] string token)
        {
            if (!db.Players.Any(i => i.Token == token))
            {
                await Clients.Caller.SendAsync("GoToLogin", "Niepoprawny token. Zaloguj się ponownie.");
                return;
            }
            //check if room with that name exists
            if (!db.Rooms.Any(from => from.Name == roomName))
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Taki pokój nie istnieje.");
                return;
            }

            Player player = db.Players.First(from => from.Token == token);
            //disconnect player from all games
            foreach (Coord c in player.CoordsList)
            {
                c.IsConnected = false;
            }

            //check if player was playing that game
            if (!player.CoordsList.Any(c => c.RoomName == roomName))
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Taki pokój nie istnieje.");
                return;
            }
            
            Coord coord = player.CoordsList.First(c => c.RoomName == roomName);
            coord.IsConnected = true;
            
            player.ConnectionID = Context.ConnectionId;
            await db.SaveChangesAsync();
            await Clients.Caller.SendAsync("JoinedRoom", roomName);
        }
        public async Task UpdateLocation([Required] double lat, [Required] double lon, [Required] string token)
        {
            if (!db.Players.Any(i => i.Token == token))
            {
                await Clients.Caller.SendAsync("GoToLogin", "Niepoprawny token. Zaloguj się ponownie.");
                return;
            }
            Player player = db.Players.First(p => p.Token == token);
            foreach (Coord c in player.CoordsList)
            {
                if (c.IsConnected == true)
                {
                    c.Latitude = lat;
                    c.Longitude = lon;
                    await db.SaveChangesAsync();

                    List<PlayerCoordsView> list = new List<PlayerCoordsView>();
                    foreach (Coord c2 in c.Room.CoordsList)
                    {
                        PlayerCoordsView view = new PlayerCoordsView(c2.PlayerName, c2.Latitude, c2.Longitude);
                        list.Add(view);
                    }
                    string json = JsonSerializer.Serialize(list);

                    //zrobic podzial wysylania na teamy!!!!!!!!!
                    await Clients.Caller.SendAsync("GetLocationFromServer", json);

                    break;
                }
            }
        }
        
        public override Task OnConnectedAsync()
        {
            var id = Context.User.Identity;
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            //disconnect player from games
            if (db.Players.Any(p => p.ConnectionID == Context.ConnectionId))
            {
                Player player = db.Players.First(p => p.ConnectionID == Context.ConnectionId);
                foreach (Coord c in player.CoordsList)
                {
                    c.IsConnected = false;
                }
                db.Players.Update(player);
                db.SaveChangesAsync();
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}

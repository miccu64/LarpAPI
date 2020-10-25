using larp_server.Models;
using larp_server.Security;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace larp_server.Hubs
{
    //[EnableCors("MyPolicy")]
    public class GameHub : Hub
    {
        private readonly GamesContext db;
        private readonly JWTWorker JWTInstance;
        public GameHub(GamesContext context)
        {
            db = context;
            JWTInstance = new JWTWorker();
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task RegisterNewUser([Required] string email, [Required] string name, [Required] string password)
        {
            if (db.Players.Any(from => from.Email == email))
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Taki e-mail już istnieje. Podaj inny.");
                return;
            }
            if (db.Players.Any(from => from.Name == name))
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Taki login już istnieje. Podaj inny.");
                return;
            }
            string token = JWTInstance.Encode(name, email);
            Player player = new Player(email, name, password, token);
            await db.AddAsync(player);
            await db.SaveChangesAsync();
            await Clients.Caller.SendAsync("SuccessMessage", "Pomyślnie zarejestrowano. Możesz się zalogować.");
        }
        public async Task Login([Required] string email, [Required] string password)
        {
            if (db.Players.Any(p => p.Email == email && p.Password == password))
            {
                Player player = db.Players.First(p => p.Email == email);
                await Clients.Caller.SendAsync("SaveToken", player.Token);
            }
            else await Clients.Caller.SendAsync("ErrorMessage", "Niepoprawny login lub hasło.");
        }
        public async Task CreateRoom([Required] string roomName, [Required] string password, [Required] string token)
        {
            if (!db.Players.Any(i => i.Token == token))
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Niepoprawny token. Zaloguj się ponownie.");
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
            await JoinRoom(roomName, password, token);
        }
        private async void AddCoord(Room room, Player player)
        {
            Coord coord = new Coord(room, player, Context.ConnectionId);
            player.CoordsList.Add(coord);
            room.CoordsList.Add(coord);
            await db.Coords.AddAsync(coord);
        }
        public async Task JoinRoom([Required] string roomName, [Required] string password, [Required] string token)
        {
            if (!db.Players.Any(i => i.Token == token))
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Niepoprawny token. Zaloguj się ponownie.");
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
            bool isNull = (player.CoordsList == null);
            //disconnect player from all games
            if (!isNull) foreach (Coord c in player.CoordsList)
            {
                c.IsConnected = false;
            }

            //check if player were playing that game
            if (isNull)
                AddCoord(room, player);
            else if (!player.CoordsList.Any(c => c.RoomId == roomName))
                AddCoord(room, player);
            else
            {
                //SPRAWDZIC CZY AKTUALIZUJAC V1 AKTUALIZUJE TEZ V2!!!
                Coord coord = player.CoordsList.First(c => c.RoomId == roomName);
                Coord coord2 = room.CoordsList.First(c => c.RoomId == roomName);
                coord.IsConnected = true;
                db.Coords.Update(coord);
            }
            player.ConnectionID = Context.ConnectionId;
            await db.SaveChangesAsync();
            await Clients.Caller.SendAsync("SuccessMessage", "Dołączono do pokoju.");
        }
        public async Task UpdateLocation([Required] double lat, [Required] double lon, [Required] string token)
        {
            if (!db.Players.Any(i => i.Token == token))
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Niepoprawny token. Zaloguj się ponownie.");
                return;
            }
            Player player = db.Players.First(p => p.Token == token);
            foreach (Coord c in player.CoordsList)
            {
                if (c.IsConnected == true)
                {
                    c.Latitude = lat;
                    c.Longitude = lon;
                    db.Coords.Update(c);
                    await db.SaveChangesAsync();
                    break;
                }
            }
        }
        /*
        public override Task OnConnectedAsync()
        {
            var id = Context.User.Identity;
            return base.OnConnectedAsync();
        }*/

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

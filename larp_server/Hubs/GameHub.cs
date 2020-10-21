using larp_server.Models;
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
        public GameHub(GamesContext context)
        {
            db = context;
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
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
            Coord coord = new Coord(room, player, Context.ConnectionId);
            player.CoordsList.Add(coord);
            room.CoordsList.Add(coord);
            await db.Coords.AddAsync(coord);
            await db.SaveChangesAsync();
            await Clients.Caller.SendAsync("SuccessMessage", "Dołączono do pokoju.");
        }
        public async Task UpdateLocation([Required] double lat, [Required] double lon, [Required] string roomName, [Required] string token)
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
            Player player = db.Players.First(p => p.Token == token);
            if (!db.Coords.Any(c => (c.RoomId == roomName && c.PlayerId == player.Name)))
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Niepoprawne dane.");
                return;
            }
            Coord coord = db.Coords.First(c => (c.RoomId == roomName && c.PlayerId == player.Name));
            coord.Latitude = lat;
            coord.Longitude = lon;
            db.Coords.Update(coord);//nie da sie async???
            await db.SaveChangesAsync();
        }

        public override Task OnConnectedAsync()
        {
            var id = Context.User.Identity;
            return base.OnConnectedAsync();
        }
    }
}

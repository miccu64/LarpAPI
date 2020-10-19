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
            if (db.Players.Any(i => i.Token == token)) 
            {
                //check if toom with that name exists
                if (!db.Rooms.Any(from => from.Name == roomName))
                {
                    Player player = db.Players.First(i => i.Token == token);
                    Room room = new Room(roomName, password, player);
                    await db.Rooms.AddAsync(room);
                    await db.SaveChangesAsync();
                    await Clients.Caller.SendAsync("SuccessMessage", "Poprawnie utworzono pokój.");
                }
                else await Clients.Caller.SendAsync("ErrorMessage", "Taki pokój już istnieje. Podaj inną nazwę.");
            }
            else await Clients.Caller.SendAsync("ErrorMessage", "Niepoprawny token. Zaloguj się ponownie.");
        }
        public async Task JoinRoom(string roomName, int team)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            if (team == 1)
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName + "-team1");
            else
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName + "-team2");
            await Clients.Group(roomName).SendAsync(Context.User.Identity.Name + "has joined.");
        }
        public async Task UpdateLocation([Bind("Id,Longitude,Latitude")] Coord coords, string roomName, int team)
        {
            //check if that id exist in db - if yes, just update
            //bool found = db.Coords.Any(from => from.Id == coords.Id);
            //Coords found = await db.Coords.FindAsync(coords);
            //if (found == false)
            {
                await db.AddAsync(coords);
                await db.SaveChangesAsync();

                await Clients.Group(roomName).SendAsync(Context.User.Identity.Name + "has joined.");
            }
        }

        public override Task OnConnectedAsync()
        {
            var id = Context.User.Identity;
            return base.OnConnectedAsync();
        }
    }
}

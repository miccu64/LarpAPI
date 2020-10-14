using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace larp_server.Hubs
{
    //[EnableCors("MyPolicy")]
    public class GameHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task CreateRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName + "-team1");
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName + "-team2");
        }
        public async Task JoinRoom(string roomName, int team)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            if (team == 0)
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName + "-team1");
            else
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName + "-team2");
            await Clients.Group(roomName).SendAsync(Context.User.Identity.Name + "has joined.");
        }
    }
}

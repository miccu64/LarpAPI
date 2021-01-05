using larp_server.Models;
using larp_server.Security;
using Microsoft.AspNetCore.SignalR;
using Server.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace larp_server.Hubs
{
    public class GameHub : Hub
    {
        private readonly GamesContext db;
        private readonly JWTWorker JWTInstance;

        public GameHub(GamesContext context)
        {
            db = context;
            JWTInstance = new JWTWorker();
        }
        private async Task<bool> CheckToken(string token)
        {
            if (!db.Players.Any(i => i.Token == token))
            {
                await Clients.Caller.SendAsync("GoToLogin", "Niepoprawny token. Zaloguj się ponownie.");
                return true;
            }
            return false;
        }

        public async Task SendMessage([Required] string message, [Required] string roomName, [Required] bool toAll, [Required] string token)
        {
            if (await CheckToken(token))
            {
                return;
            }
            Player player = db.Players.First(p => p.Token == token);
            if (!db.Coords.Any(c => c.PlayerName == player.Nickname && c.RoomName == roomName && c.IsConnected == true))
            {
                await Clients.Caller.SendAsync("ShowMessage", "Niepoprawne dane.");
                return;
            }

            Coord coord = db.Coords.First(c => c.PlayerName == player.Nickname && c.RoomName == roomName && c.IsConnected == true);
            List<string> userIds = new List<string>();
            int team = coord.TeamId;

            foreach (Coord c2 in coord.Room.CoordsList)
            {
                if ((c2.IsConnected == true) && (c2.TeamId == team || toAll == true))
                {
                    userIds.Add(c2.Player.ConnectionID);
                }
            }

            string who = player.Nickname;
            if (toAll == true)
            {
                who += " (wszyscy): ";
            }
            else who += ": ";
            await Clients.Clients(userIds).SendAsync("GetChatMessage", who + message);
        }
        public async Task RegisterNewUser([Required] string email, [Required] string name, [Required] string password)
        {
            if (name.Length < 4 || password.Length < 4 || email.Length < 4)
            {
                return;
            }
            if (db.Players.Any(from => from.Email == email))
            {
                await Clients.Caller.SendAsync("ShowMessage", "Taki e-mail już istnieje. Podaj inny.");
                return;
            }
            if (db.Players.Any(from => from.Nickname == name))
            {
                await Clients.Caller.SendAsync("ShowMessage", "Taki login już istnieje. Podaj inny.");
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
            if (password.Length < 4 || email.Length < 4)
            {
                return;
            }
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
            else await Clients.Caller.SendAsync("ShowMessage", "Niepoprawny login lub hasło.");
        }
        public async Task CreateRoom([Required] string roomName, [Required] string password, [Required] int team, [Required] string token)
        {
            if (roomName.Length < 4 || password.Length < 4)
            {
                return;
            }
            if (await CheckToken(token))
            {
                return;
            }
            if (db.Rooms.Any(from => from.Name == roomName))
            {
                await Clients.Caller.SendAsync("ShowMessage", "Taki pokój już istnieje. Podaj inną nazwę.");
                return;
            }

            Player player = db.Players.First(i => i.Token == token);
            Room room = new Room(roomName, password, player);
            await db.Rooms.AddAsync(room);
            await db.SaveChangesAsync();

            await JoinRoom(roomName, password, team, token);
        }
        private async Task AddCoord(Room room, Player player, int team)
        {
            Coord coord = new Coord(room, player, team);
            player.CoordsList.Add(coord);
            room.CoordsList.Add(coord);
            await db.Coords.AddAsync(coord);
        }
        public async Task JoinRoom([Required] string roomName, [Required] string password, [Required] int team, [Required] string token)
        {
            if (await CheckToken(token))
            {
                return;
            }
            //check if room with that name exists
            if (!db.Rooms.Any(from => from.Name == roomName))
            {
                await Clients.Caller.SendAsync("ShowMessage", "Taki pokój nie istnieje.");
                return;
            }
            //check passwords
            Room room = db.Rooms.First(from => from.Name == roomName);
            if (room.Password != password)
            {
                await Clients.Caller.SendAsync("ShowMessage", "Niepoprawne hasło.");
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
            {
                await AddCoord(room, player, team);
            }
            else
            {
                Coord coord = player.CoordsList.First(c => c.RoomName == roomName);
                coord.IsConnected = true;
            }
            player.ConnectionID = Context.ConnectionId;
            await db.SaveChangesAsync();
            await Clients.Caller.SendAsync("JoinedRoom", roomName);
        }
        public async Task JoinJoinedRoom([Required] string roomName, [Required] bool lostConnection, [Required] string token)
        {
            if (await CheckToken(token))
            {
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
                await Clients.Caller.SendAsync("ShowMessage", "Niepoprawna nazwa pokoju.");
                return;
            }

            Coord coord = player.CoordsList.First(c => c.RoomName == roomName);
            coord.IsConnected = true;

            player.ConnectionID = Context.ConnectionId;
            await db.SaveChangesAsync();
            //if disconnected and rejoined - don't send message
            if (lostConnection == false)
            {
                await Clients.Caller.SendAsync("JoinedRoom", roomName);
            }
        }
        public async Task UpdateLocation([Required] double lat, [Required] double lon, [Required] string token)
        {
            if (await CheckToken(token))
            {
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
                    break;
                }
            }
        }
        public async Task DeleteRoom([Required] string roomName, [Required] string token)
        {
            if (!db.Rooms.Any(i => i.Name == roomName))
            {
                return;
            }
            Room room = db.Rooms.First(i => i.Name == roomName);
            if (room.Admin.Token != token)
            {
                await Clients.Caller.SendAsync("ShowMessage", "Nie jesteś adminem.");
                return;
            }
            foreach (Coord c in room.CoordsList)
            {
                Player p = c.Player;
                p.CoordsList.Remove(c);
                db.Coords.Remove(c);
            }
            room.CoordsList.Clear();
            db.Rooms.Remove(room);
            await db.SaveChangesAsync();
            await Clients.Caller.SendAsync("GoToLogin", "Pomyślnie usunięto pokój.");
        }
        public async Task LeaveRoom([Required] string roomName, [Required] string token)
        {
            if (await CheckToken(token))
            {
                return;
            }
            Player player = db.Players.First(p => p.Token == token);
            foreach (Coord c in player.CoordsList)
            {
                if (c.RoomName != roomName)
                {
                    continue;
                }
                //check if player is admin
                Room room = c.Room;
                db.Coords.Remove(c);
                if (room.Admin.Token == token)
                {
                    if (room.CoordsList.Count == 1)
                    {
                        db.Rooms.Remove(room);
                        await db.SaveChangesAsync();
                        await Clients.Caller.SendAsync("GoToLogin", "Pomyślnie usunięto pokój.");
                        return;
                    }
                    //give admin to other player
                    room.Admin = room.CoordsList.First(c => c.PlayerName != room.Admin.Nickname).Player;
                }
                await db.SaveChangesAsync();
                await Clients.Caller.SendAsync("GoToLogin", "Pomyślnie opuściłeś pokój.");
                return;
            }
        }
        public async Task GiveAdmin([Required] string roomName, [Required] string playerName, [Required] string token)
        {
            if (!db.Rooms.Any(i => i.Name == roomName))
            {
                return;
            }
            Room room = db.Rooms.First(r => r.Name == roomName);
            if (room.Admin.Token != token)
            {
                await Clients.Caller.SendAsync("ShowMessage", "Nie jesteś adminem.");
                return;
            }
            foreach (Coord c in room.CoordsList)
            {
                if (c.PlayerName == playerName)
                {
                    room.Admin = db.Players.First(p => p.Nickname == playerName);
                    await db.SaveChangesAsync();
                    await Clients.Caller.SendAsync("ShowMessage", "Pomyślnie przekazano prawa admina dla " + playerName);
                    return;
                }
            }
            await Clients.Caller.SendAsync("ShowMessage", "Niepoprawny nick gracza.");
        }
        public async Task ThrowPlayer([Required] string roomName, [Required] string playerName, [Required] string token)
        {
            if (!db.Rooms.Any(i => i.Name == roomName))
            {
                return;
            }
            Room room = db.Rooms.First(r => r.Name == roomName);
            if (room.Admin.Token != token)
            {
                await Clients.Caller.SendAsync("ShowMessage", "Nie jesteś adminem.");
                return;
            }
            foreach (Coord c in room.CoordsList)
            {
                if (c.PlayerName != playerName)
                {
                    continue;
                }
                Player playerToThrow = c.Player;
                if (playerToThrow.Nickname == playerName)
                {
                    await LeaveRoom(roomName, token);
                    return;
                }
                if (c.IsConnected)
                {
                    await Clients.Client(playerToThrow.ConnectionID).SendAsync("GoToLogin", "Zostałeś wyrzucony z pokoju.");
                }
                db.Coords.Remove(c);
                await db.SaveChangesAsync();
                return;
            }
            await Clients.Caller.SendAsync("ShowMessage", "Nie ma takiego gracza w grze.");
        }
        public override Task OnConnectedAsync()
        {
            CountPlayers.ConnectedPlayers++;
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
                db.SaveChangesAsync();
            }
            CountPlayers.ConnectedPlayers--;
            return base.OnDisconnectedAsync(exception);
        }
    }
}

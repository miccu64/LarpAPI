using larp_server.Models;
using larp_server.Views;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace larp_server.Hubs
{
    public class HubTimerWorker : BackgroundService
    {
        private readonly IHubContext<GameHub> _hub;
        IServiceScopeFactory _serviceScopeFactory;

        public HubTimerWorker(IServiceScopeFactory serviceScopeFactory, IHubContext<GameHub> hub)
        {
            _hub = hub;
            //needed to get the database
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _db = scope.ServiceProvider.GetService<GamesContext>();
                    var rooms = _db.Rooms.ToList();//.ToListAsync();
                    foreach (Room r in rooms)
                    {
                        List<string>[] userIdsByTeam = new List<string>[4];
                        List<PlayerCoordsView>[] coordsByTeam = new List<PlayerCoordsView>[4];
                        for (int a = 0; a < 4; a++)
                        {
                            userIdsByTeam[a] = new List<string>();
                            coordsByTeam[a] = new List<PlayerCoordsView>();
                        }
                        foreach (Coord c in r.CoordsList)
                        {
                            if (c.TeamId > 0 && c.TeamId < 5)
                            {
                                if (c.IsConnected)
                                {
                                    userIdsByTeam[c.TeamId - 1].Add(c.Player.ConnectionID);
                                }
                                coordsByTeam[c.TeamId - 1].Add(new PlayerCoordsView(c));
                            }
                        }
                        for (int a = 0; a < 4; a++)
                        {
                            if (userIdsByTeam[a].Count > 0)
                            {
                                string json = JsonSerializer.Serialize(coordsByTeam[a]);
                                await _hub.Clients.Clients(userIdsByTeam[a]).SendAsync("SuccessMessage", json);
                                await _hub.Clients.Clients(userIdsByTeam[a]).SendAsync("GetLocationFromServer", json);
                            }
                            
                        }
                    }
                }
                await Task.Delay(2000);
            }
        }
    }
}

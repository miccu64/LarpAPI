using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Server.Models;
using System.Linq;
using larp_server.Models;

namespace larp_server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<GamesContext>();
                    //disconnect all players when restarting server
                    var players = context.Players.ToList();
                    foreach (Player p in players)
                    {
                        foreach (Coord c in p.CoordsList)
                        {
                            c.IsConnected = false;
                        }
                    }
                    context.SaveChanges();
                }
                catch (Exception ex)
                { }
            }

            host.Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using larp_server.Models;
using larp_server.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Inzynierka_Serwer.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class UserController : Controller
    {
        private readonly GamesContext db;
        private readonly JWTWorker JWTInstance;
        public UserController(GamesContext context)
        {
            db = context;
            JWTInstance = new JWTWorker();
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.Coords.ToListAsync());
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterNewUser([Required] string email, [Required] string name, [Required] string password)
        {
            if (db.Players.Any(from => from.Email == email || from.Name == name))
                return BadRequest("Taki e-mail lub login już istnieje. Podaj inny.");
            string token = JWTInstance.Encode(name, email);
            Player player = new Player(email, name, password, token);
            await db.AddAsync(player);
            await db.SaveChangesAsync();
            return Ok("Pomyślnie zarejestrowano. Możesz się zalogować.");
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login([Required] string email, [Required] string password)
        {
            if(db.Players.Any(p => p.Email == email))
            {
                Player player = await db.Players.FirstOrDefaultAsync(p => p.Email == email);
                if (player.Password == password)
                    return Ok(player.Token);
                else return BadRequest("Niepoprawny login lub hasło.");
            } 
            else return BadRequest("Niepoprawny login lub hasło.");
        }
    }
}

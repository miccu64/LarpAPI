using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using larp_server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Inzynierka_Serwer.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class CoordsController : Controller
    {
        private readonly CoordsContext db;

        public CoordsController(CoordsContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.Coords.ToListAsync());
        }

        [HttpGet("getById")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coords = await db.Coords
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coords == null)
            {
                return NotFound();
            }

            return View(coords);
        }

        // POST: Coords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("register")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Longitude,Latitude")] Coords coords)
        {
            if (ModelState.IsValid)
            {
                //check if that id exist in db - if yes, just update
                bool found = db.Coords.Any(from => from.Id == coords.Id);
                //Coords found = await db.Coords.FindAsync(coords);
                if (found == false)
                {
                    await db.AddAsync(coords);
                    await db.SaveChangesAsync();
                    return Ok("Pomyślnie dodano.");
                }
                return Ok("Już istnieje taki wpis.");
            }
            return BadRequest("Niepoprawne dane.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> Edit([Bind("Id,Longitude,Latitude")] Coords coords)
        {
            if (ModelState.IsValid)
            {
                bool found = db.Coords.Any(from => from.Id == coords.Id);
                if (!found)
                {
                    return NotFound();
                }

                try
                {
                    db.Update(coords);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoordsExists(coords.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok("oki");
            }
            return BadRequest("Niepoprawne dane.");
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var coords = await db.Coords.FindAsync(id);
            db.Coords.Remove(coords);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoordsExists(string id)
        {
            return db.Coords.Any(e => e.Id == id);
        }

        /*
        // GET: Coords/Create
        public IActionResult Create()
        {
            return View();
        }*/
        /*
        // GET: Coords/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coords = await db.Coords.FindAsync(id);
            if (coords == null)
            {
                return NotFound();
            }
            return View(coords);
        }*/
        /*
        // GET: Coords/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coords = await db.Coords
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coords == null)
            {
                return NotFound();
            }

            return View(coords);
        }*/
    }
}

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
        private readonly CoordsContext _context;

        public CoordsController(CoordsContext context)
        {
            _context = context;
        }

        // GET: Coords
        public async Task<IActionResult> Index()
        {
            return View(await _context.Coords.ToListAsync());
        }

        // GET: Coords/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coords = await _context.Coords
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coords == null)
            {
                return NotFound();
            }

            return View(coords);
        }

        // GET: Coords/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("send")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Longitude,Latitude")] Coords coords)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coords);
                _context.SaveChanges();
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coords);
        }

        // GET: Coords/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coords = await _context.Coords.FindAsync(id);
            if (coords == null)
            {
                return NotFound();
            }
            return View(coords);
        }

        // POST: Coords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Longitude,Latitude")] Coords coords)
        {
            if (id != coords.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coords);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            return View(coords);
        }

        // GET: Coords/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coords = await _context.Coords
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coords == null)
            {
                return NotFound();
            }

            return View(coords);
        }

        // POST: Coords/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var coords = await _context.Coords.FindAsync(id);
            _context.Coords.Remove(coords);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoordsExists(string id)
        {
            return _context.Coords.Any(e => e.Id == id);
        }
    }
}

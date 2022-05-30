using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameStore;

namespace GameStore.Controllers
{
    public class GamesController : Controller
    {
        private readonly GamesDBContext _context;

        public GamesController(GamesDBContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index()
        {
            var gamesDBContext = _context.Games.Include(g => g.GameStudio);
            return View(await gamesDBContext.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? itemId)
        {
            if (itemId == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.GameStudio)
                .FirstOrDefaultAsync(m => m.GameId == itemId);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            ViewData["GameStudioId"] = new SelectList(_context.GameStudios, "Id", "Id");
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GameId,Name,Description,GameStudioId,AgePermission,Price,Genre")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameStudioId"] = new SelectList(_context.GameStudios, "Id", "Id", game.GameStudioId);
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? itemId)
        {
            if (itemId == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(itemId);
            if (game == null)
            {
                return NotFound();
            }
            ViewData["GameStudioId"] = new SelectList(_context.GameStudios, "Id", "Id", game.GameStudioId);
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int itemId, [Bind("GameId,Name,Description,GameStudioId,AgePermission,Price,Genre")] Game game)
        {
            if (itemId != game.GameId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.GameId))
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
            ViewData["GameStudioId"] = new SelectList(_context.GameStudios, "Id", "Id", game.GameStudioId);
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? itemId)
        {
            if (itemId == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.GameStudio)
                .FirstOrDefaultAsync(m => m.GameId == itemId);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int itemId)
        {
            if (_context.Games == null)
            {
                return Problem("Entity set 'GamesDBContext.Games'  is null.");
            }
            var game = await _context.Games.FindAsync(itemId);
            if (game != null)
            {
                _context.Games.Remove(game);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
          return (_context.Games?.Any(e => e.GameId == id)).GetValueOrDefault();
        }
    }
}

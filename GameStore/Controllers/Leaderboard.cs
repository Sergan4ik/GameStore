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
    public class Leaderboard : Controller
    {
        private readonly GamesDBContext _context;

        public Leaderboard(GamesDBContext context)
        {
            _context = context;
        }

        // GET: Leaderboard
        public async Task<IActionResult> Index(string myUsername)
        {
            ViewBag.myUsername = myUsername;
            var orderedUsers = await _context.Users
                .Include(u => u.GameCopies).ThenInclude(gc => gc.Game)
                .Include(u => u.ItemsInInventories).ThenInclude(i => i.SourceItem)
                .OrderByDescending(u =>
                    (u.GameCopies.Sum(gc => gc.Game.Price).Value + u.ItemsInInventories.Sum(i => i.SourceItem.Price)))
                .ThenBy(u => u.Username).ToListAsync();
            return View(orderedUsers);
        }

        // GET: Leaderboard/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = _context.Users
                .Include(u => u.GameCopies).ThenInclude(gc => gc.Game).ThenInclude(g => g.GameStudio)
                .First(u => u.Id == id);
            ViewBag.user = user.Username;
            return View(user.GameCopies);
        }

        // GET: Leaderboard/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leaderboard/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Email,BirthDate,Balance")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Leaderboard/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Leaderboard/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Email,BirthDate,Balance")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Leaderboard/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Leaderboard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'GamesDBContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameStore;

namespace GameStore.Controllers
{
    public class UsersController : Controller
    {
        private readonly GamesDBContext _context;

        public UsersController(GamesDBContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
              return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'GamesDBContext.Users'  is null.");
        }

        // GET: Users/Details/5
        [HttpGet("Details")]
        public async Task<IActionResult> Details(int? id)
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

            return RedirectToAction("Index", "GamesCopy", new { userId = user.Id, username = user.Username });
        }

        // GET: Users/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
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

        // GET: Users/Edit/5
        [HttpGet("Edit")]
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

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int itemId, [Bind("Id,Username,Email,BirthDate,Balance")] User user)
        {
            if (itemId != user.Id)
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

        // GET: Users/Delete/5
        [HttpGet("Delete")]
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

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int itemId)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'GamesDBContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(itemId);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid && fileExcel != null)
            {
                await using var stream = new FileStream(fileExcel.FileName, FileMode.Create);
                await fileExcel.CopyToAsync(stream);
                using XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled);
                foreach (IXLWorksheet worksheet in workBook.Worksheets)
                {
                    Game game = _context.Games.FirstOrDefault(g => g.Name == worksheet.Name);
                    if (game == null)
                        continue;
                    ParseItemsSheet(worksheet, game);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private void ParseItemsSheet(IXLWorksheet worksheet, Game game)
        {
            foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                ParseSingleItem(game, row);
        }

        private void ParseSingleItem(Game game, IXLRow row)
        {
            try
            {
                Item item = new Item();
                item.Name = row.Cell(1).Value.ToString();

                if (item.Name == "" || _context.Items.Any(i => i.Name == item.Name))
                    return;

                item.Price = decimal.Parse(row.Cell(2).Value.ToString());
                item.Rarity = row.Cell(3).Value.ToString();
                item.Game = game.GameId;
                item.GameNavigation = game;
                _context.Items.Add(item);
            }
            catch (Exception e)
            {
            }
        }
        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

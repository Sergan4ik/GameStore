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
    public class ItemsInventory : Controller
    {
        private readonly GamesDBContext _context;

        public ItemsInventory(GamesDBContext context)
        {
            _context = context;
        }

        // GET: ItemsInventory
        public async Task<IActionResult> Index(string email)
        {
            var user = _context.GetUserByEmail(email);
            var gamesDBContext = _context.ItemsInInventories
                .Include(i => i.Owner)
                .Include(i => i.SourceItem)
                .ThenInclude(i => i.GameNavigation)
                .Where(i => i.OwnerId == user.Id);
            
            return View(await gamesDBContext.ToListAsync());
        }

        // GET: ItemsInventory/Details/5
        public async Task<IActionResult> Details(int? itemId)
        {
            if (itemId == null || _context.ItemsInInventories == null)
            {
                return NotFound();
            }

            var itemsInInventory = await _context.ItemsInInventories
                .Include(i => i.Owner)
                .Include(i => i.SourceItem)
                .FirstOrDefaultAsync(m => m.ItemCopyId == itemId);
            if (itemsInInventory == null)
            {
                return NotFound();
            }

            return View(itemsInInventory);
        }

        // GET: ItemsInventory/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["SourceItemId"] = new SelectList(_context.Items, "Id", "Id");
            return View();
        }

        // POST: ItemsInventory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemCopyId,SourceItemId,OwnerId")] ItemsInInventory itemsInInventory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(itemsInInventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", itemsInInventory.OwnerId);
            ViewData["SourceItemId"] = new SelectList(_context.Items, "Id", "Id", itemsInInventory.SourceItemId);
            return View(itemsInInventory);
        }

        // GET: ItemsInventory/Edit/5
        public async Task<IActionResult> Edit(int? itemId)
        {
            if (itemId == null || _context.ItemsInInventories == null)
            {
                return NotFound();
            }

            var itemsInInventory = await _context.ItemsInInventories.FindAsync(itemId);
            if (itemsInInventory == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", itemsInInventory.OwnerId);
            ViewData["SourceItemId"] = new SelectList(_context.Items, "Id", "Id", itemsInInventory.SourceItemId);
            return View(itemsInInventory);
        }

        // POST: ItemsInventory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int itemId, [Bind("ItemCopyId,SourceItemId,OwnerId")] ItemsInInventory itemsInInventory)
        {
            if (itemId != itemsInInventory.ItemCopyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemsInInventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemsInInventoryExists(itemsInInventory.ItemCopyId))
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
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", itemsInInventory.OwnerId);
            ViewData["SourceItemId"] = new SelectList(_context.Items, "Id", "Id", itemsInInventory.SourceItemId);
            return View(itemsInInventory);
        }

        // GET: ItemsInventory/Delete/5
        public async Task<IActionResult> Delete(int? itemId)
        {
            if (itemId == null || _context.ItemsInInventories == null)
            {
                return NotFound();
            }

            var itemsInInventory = await _context.ItemsInInventories
                .Include(i => i.Owner)
                .Include(i => i.SourceItem)
                .FirstOrDefaultAsync(m => m.ItemCopyId == itemId);
            if (itemsInInventory == null)
            {
                return NotFound();
            }
            

            return View(itemsInInventory);
        }

        // POST: ItemsInventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int itemId)
        {
            if (_context.ItemsInInventories == null)
            {
                return Problem("Entity set 'GamesDBContext.ItemsInInventories'  is null.");
            }

            var itemsInInventory = await _context.ItemsInInventories.Include(i => i.SourceItem)
                .FirstOrDefaultAsync(i => i.ItemCopyId == itemId);
            if (itemsInInventory != null)
            {
                var user = _context.GetUserByEmail(User.Identity.Name);
                user.Balance += itemsInInventory.SourceItem.Price;
                _context.ItemsInInventories.Remove(itemsInInventory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index) , new{email = User.Identity.Name});
        }

        private bool ItemsInInventoryExists(int id)
        {
          return (_context.ItemsInInventories?.Any(e => e.ItemCopyId == id)).GetValueOrDefault();
        }
    }
}

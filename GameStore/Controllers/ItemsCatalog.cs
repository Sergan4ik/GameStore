using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameStore;
using Microsoft.EntityFrameworkCore.Query;

namespace GameStore.Controllers
{
    public class ItemsCatalog : Controller
    {
        private readonly GamesDBContext _context;

        public ItemsCatalog(GamesDBContext context)
        {
            _context = context;
        }

        // GET: ItemsCatalog
        public async Task<IActionResult> Index(string email, string gameName = null)
        {
            var user = _context.GetUserByEmail(email);
            var gamesDBContext = _context.Items.Include(i => i.GameNavigation).Where(g =>
                gameName == null || g.GameNavigation.Name == gameName);
            
            IIncludableQueryable<ItemsInInventory, Item?> itemsInInventories = _context.ItemsInInventories
                .Where(inv => inv.OwnerId == user.Id).Include(g => g.Owner).Include(g => g.SourceItem);
            ViewBag.ByedItems = itemsInInventories;
            var selectList = new SelectList(_context.Games ,"Name" , "Name" , gameName);
            ViewBag.Games = selectList;
            ViewBag.Balance = user.Balance.Value;            
            return View(await gamesDBContext.ToListAsync());
        }

        // GET: ItemsCatalog/Details/5
        public async Task<IActionResult> Details(int? itemId)
        {
            if (itemId == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.GameNavigation)
                .FirstOrDefaultAsync(m => m.Id == itemId);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: ItemsCatalog/Edit/5
        public async Task<IActionResult> Edit(int? itemId, string email)
        {
            if (itemId == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId);

            if (item == null)
                return NotFound();

            ViewBag.Balance = _context.GetUserByEmail(email).Balance.Value;
            ViewBag.ItemCost = item.Price;
            return View(item);
        }

        // POST: ItemsCatalog/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyItem(int itemId, string userName)
        {
            if (itemId == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId);
            var user = _context.GetUserByEmail(userName);
            if (item == null)
                return NotFound();
            if (user.Balance < item.Price)
                return NotFound();
            
            user.Balance -= item.Price;
            _context.ItemsInInventories.Add(new ItemsInInventory()
            {
                OwnerId = user.Id,
                SourceItemId = item.Id
            });
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index" , new {email = User.Identity.Name});
        }

        private bool ItemExists(int id)
        {
          return (_context.Items?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

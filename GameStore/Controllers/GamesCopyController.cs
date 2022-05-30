using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameStore;

namespace GameStore.Controllers
{
    public class GamesCopyController : Controller
    {
        private readonly GamesDBContext _context;

        public GamesCopyController(GamesDBContext context)
        {
            _context = context;
        }

        // GET: GamesCopy
        
        public async Task<IActionResult> Index(int? userId , string? username)
        {
            if (userId == null) return RedirectToAction("Index", "Home");

            ViewBag.Id = userId;
            ViewBag.Username = username;
            ViewBag.Balance = _context.GetUserByEmail(User.Identity.Name).Balance.Value;
            var byedGames = _context.GameCopies
                .Where(g => g.UserId == userId)
                .Include(g => g.User)
                .Include(g => g.Game)
                .Include(g => g.Game)
                .ThenInclude(g => g.GameStudio);
            return View(await byedGames.ToListAsync());
        }

        // GET: GamesCopy/Details/5

        public async Task<IActionResult> Details(int? itemId)
        {
            if (itemId == null || _context.GameCopies == null)
            {
                return NotFound();
            }

            var gameCopy = await _context.GameCopies
                .Include(g => g.Game)
                .ThenInclude(g => g.GameStudio)
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.CopyId == itemId);
            if (gameCopy == null)
            {
                return NotFound();
            }

            return View(gameCopy);
        }

        // GET: GamesCopy/Create

        public IActionResult Create(int UserId)
        {
            ViewBag.UserId = UserId;
            ViewBag.Username = _context.Users.First(u => u.Id == UserId).Username;
            ViewBag.Games = new SelectList(_context.Games, "GameId" , "Name");
            return View();
        }

        // POST: GamesCopy/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int UserId, [Bind("CopyId,UserId,GameId,BuyDate")] GameCopy gameCopy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gameCopy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameId", gameCopy.GameId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", gameCopy.UserId);
            return View(gameCopy);
        }

        // GET: GamesCopy/Edit/5
        public async Task<IActionResult> Edit(int? itemId)
        {
            if (itemId == null || _context.GameCopies == null)
            {
                return NotFound();
            }

            var gameCopy = await _context.GameCopies.Include(g => g.Game).Include(g => g.User).FirstAsync(g => g.CopyId == itemId);
            if (gameCopy == null)
            {
                return NotFound();
            }
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "Name", gameCopy.Game.Name);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", gameCopy.User.Username);
            return View(gameCopy);
        }

        // POST: GamesCopy/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int itemId, [Bind("CopyId,UserId,GameId,BuyDate")] GameCopy gameCopy)
        {
            if (itemId != gameCopy.CopyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gameCopy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameCopyExists(gameCopy.CopyId))
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
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameId", gameCopy.GameId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", gameCopy.UserId);
            return View(gameCopy);
        }

        // GET: GamesCopy/Delete/5
        public async Task<IActionResult> Delete(int? itemId)
        {
            if (itemId == null || _context.GameCopies == null)
            {
                return NotFound();
            }

            var gameCopy = await _context.GameCopies
                .Include(g => g.Game)
                .ThenInclude(g => g.GameStudio)
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.CopyId == itemId);
            if (gameCopy == null)
            {
                return NotFound();
            }

            return View(gameCopy);
        }

        // POST: GamesCopy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int itemId)
        {
            if (_context.GameCopies == null)
            {
                return Problem("Entity set 'GamesDBContext.GameCopies'  is null.");
            }

            var userByEmail = _context.GetUserByEmail(User.Identity.Name);
            var gameCopy = await _context.GameCopies.Include(g => g.Game)
                .FirstAsync(g => g.CopyId == itemId);
            if (gameCopy != null)
            {
                userByEmail.Balance += gameCopy.Game.Price;
                _context.GameCopies.Remove(gameCopy);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Export(int userId)
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var byedGames = _context.GameCopies
                    .Include(g => g.Game)
                    .ThenInclude(g => g.GameStudio)
                    .Where(g => g.UserId == userId);
                var byedItems = _context.ItemsInInventories
                    .Include(i => i.SourceItem)
                    .ThenInclude(i => i.GameNavigation)
                    .Where(i => i.OwnerId == userId);
                var username = _context.Users.First(u => u.Id == userId).Username;
                FillExcelTableWithGames(workbook, byedGames , username);
                
                var worksheet = workbook.Worksheets.Add("Предмети");

                worksheet.Cell("A1").Value = "Предмет";
                worksheet.Cell("B1").Value = "Гра";
                worksheet.Cell("C1").Value = "Рідкість";
                worksheet.Cell("D1").Value = "Ціна";
                worksheet.Row(1).Style.Font.Bold = true;
                int curRow = 2;
                foreach (var c in byedItems)
                {
                    var books = c;
                    worksheet.Cell(curRow, 1).Value = c.SourceItem.Name;
                    worksheet.Cell(curRow, 2).Value = c.SourceItem.GameNavigation.Name;
                    worksheet.Cell(curRow, 3).Value = c.SourceItem.Rarity;
                    worksheet.Cell(curRow, 4).Value = c.SourceItem.Price.Value.ToString("0.00") + "$";
                    curRow++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        //змініть назву файла відповідно до тематики Вашого проєкту

                        FileDownloadName = $"library_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }

        private static void FillExcelTableWithGames(XLWorkbook workbook, IQueryable<GameCopy> byedGames , string username)
        {
            var worksheet = workbook.Worksheets.Add("Ігри");
            worksheet.Range("A1:E1").Merge().Value = $"Ігри куплені гравцем {username}";
            worksheet.Cell("A2").Value = "Гра";
            worksheet.Cell("B2").Value = "Розробник";
            worksheet.Cell("C2").Value = "Дата покупки";
            worksheet.Cell("D2").Value = "Жанр";
            worksheet.Cell("E2").Value = "Ціна";
            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Row(2).Style.Font.Bold = true;
            int curRow = 3;
            decimal? summaryPrice = 0;
            foreach (var c in byedGames)
            {
                var books = c;
                worksheet.Cell(curRow, 1).Value = c.Game.Name;
                worksheet.Cell(curRow, 2).Value = c.Game.GameStudio.StudioName;
                worksheet.Cell(curRow, 3).Value = c.BuyDate.Value.ToString("MM/dd/yyyy");
                worksheet.Cell(curRow, 4).Value = c.Game.Genre;
                worksheet.Cell(curRow, 5).Value = c.Game.Price.Value.ToString("0.00") + "$";
                summaryPrice += c.Game.Price;
                curRow++;
            }

            worksheet.Cell(curRow, 1).Value = "Підсумок :";
            worksheet.Cell(curRow, 3).Value = $"Куплено ігор - {byedGames.Count()}";
            worksheet.Cell(curRow, 5).Value = $"На ціну - {summaryPrice.Value.ToString("0.00")}$";
            
        }


        private bool GameCopyExists(int id)
        {
          return (_context.GameCopies?.Any(e => e.CopyId == id)).GetValueOrDefault();
        }
    }
}

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
            if (userId == null) return RedirectToAction("Index", "Users");

            ViewBag.Id = userId;
            ViewBag.Username = username;
            var byedGames = _context.GameCopies
                .Where(g => g.UserId == userId)
                .Include(g => g.User)
                .Include(g => g.Game);
            return View(await byedGames.ToListAsync());
        }

        // GET: GamesCopy/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GameCopies == null)
            {
                return NotFound();
            }

            var gameCopy = await _context.GameCopies
                .Include(g => g.Game)
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.CopyId == id);
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GameCopies == null)
            {
                return NotFound();
            }

            var gameCopy = await _context.GameCopies.Include(g => g.Game).Include(g => g.User).FirstAsync(g => g.CopyId == id);
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
        public async Task<IActionResult> Edit(int id, [Bind("CopyId,UserId,GameId,BuyDate")] GameCopy gameCopy)
        {
            if (id != gameCopy.CopyId)
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GameCopies == null)
            {
                return NotFound();
            }

            var gameCopy = await _context.GameCopies
                .Include(g => g.Game)
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.CopyId == id);
            if (gameCopy == null)
            {
                return NotFound();
            }

            return View(gameCopy);
        }

        // POST: GamesCopy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GameCopies == null)
            {
                return Problem("Entity set 'GamesDBContext.GameCopies'  is null.");
            }
            var gameCopy = await _context.GameCopies.FindAsync(id);
            if (gameCopy != null)
            {
                _context.GameCopies.Remove(gameCopy);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Export(int userId)
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                //var categories = _context.Categories.Include("Books").ToList();
                //тут, для прикладу ми пишемо усі книжки з БД, в своїх проєктах ТАК НЕ РОБИТИ (писати лише вибрані)
                //foreach (var c in categories)
                //{
                //    var worksheet = workbook.Worksheets.Add(c.Name);
                //
                //    worksheet.Cell("A1").Value = "Назва";
                //    worksheet.Cell("B1").Value = "Автор 1";
                //    worksheet.Cell("C1").Value = "Автор 2";
                //    worksheet.Cell("D1").Value = "Автор 3";
                //    worksheet.Cell("E1").Value = "Автор 4";
                //    worksheet.Cell("F1").Value = "Категорія";
                //    worksheet.Cell("G1").Value = "Інформація";
                //    worksheet.Row(1).Style.Font.Bold = true;
                //    var books = c.Books.ToList();
                //
                //    //нумерація рядків/стовпчиків починається з індекса 1 (не 0)
                //    for (int i = 0; i < books.Count; i++)
                //    {
                //        worksheet.Cell(i + 2, 1).Value = books[i].Name;
                //        worksheet.Cell(i + 2, 7).Value = books[i].Info;
                //
                //        var ab = _context.AuthorBooks.Where(a => a.BookId == books[i].Id).Include("Author").ToList();
                //       
                //
                //        //більше 4-ох нікуди писати
                //        int j = 0;
                //        foreach (var a in ab)
                //        {
                //            if (j < 5)
                //            {
                //                worksheet.Cell(i + 2, j + 2).Value = a.Author.Name;
                //                j++;
                //            }
                //        }
                //
                //    }
            }

            using (var stream = new MemoryStream())
            {
                //workbook.SaveAs(stream);
                stream.Flush();

                return new FileContentResult(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    //змініть назву файла відповідно до тематики Вашого проєкту

                    FileDownloadName = $"library_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                };
            }
        }


        private bool GameCopyExists(int id)
        {
          return (_context.GameCopies?.Any(e => e.CopyId == id)).GetValueOrDefault();
        }
    }
}

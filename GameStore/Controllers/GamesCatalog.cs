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
    public class GamesCatalog : Controller
    {
        private readonly GamesDBContext _context;

        public GamesCatalog(GamesDBContext context)
        {
            _context = context;
        }

        // GET: GamesCatalog
        public async Task<IActionResult> Index(string email , bool wasErrorWithAge = false)
        {
            var gamesDBContext = _context.Games.Include(g => g.GameStudio);
            var user = _context.GetUserByEmail(email);
            ViewBag.Balance = _context.GetUserByEmail(email).Balance.Value;
            ViewBag.ErrorWithAge = wasErrorWithAge;
            IIncludableQueryable<GameCopy,Game?> includableQueryable = _context.GameCopies.Where(g => g.UserId == user.Id).Include(g => g.Game);
            ViewBag.ByedGames = includableQueryable;
            return View(await gamesDBContext.ToListAsync());
        }

        // GET: GamesCatalog/Details/5
        public async Task<IActionResult> Details(int? itemId , string email)
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

            ViewBag.Balance = _context.GetUserByEmail(email).Balance.Value;
            ViewBag.GameCost = game.Price;

            return View(game);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyGame(int? gameId , string email)
        {
            var game = await _context.Games
                .Include(g => g.GameStudio)
                .FirstOrDefaultAsync(m => m.GameId == gameId);

            var userByEmail = _context.GetUserByEmail(email);
            
            
            var years = GetYearsBetweenTwoDates(userByEmail);

            if (years >= game.AgePermission)
            {
                userByEmail.Balance -= game.Price;
                _context.GameCopies.Add(new GameCopy()
                {
                    GameId = gameId,
                    UserId = userByEmail.Id,
                    BuyDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                ViewBag.ErrorWithAge = false;
            }
            else
            {
                ViewBag.ErrorWithAge = true;
            }

            return RedirectToAction("Index" , new {email = email, wasErrorWithAge = ViewBag.ErrorWithAge});
        }

        private int GetYearsBetweenTwoDates(User userByEmail)
        {
            DateTime zeroTime = new DateTime(1, 1, 1);
            
            TimeSpan span = DateTime.Now - userByEmail.BirthDate.Value;
            int years = (zeroTime + span).Year - 1;
            return years;
        }

        private bool GameExists(int id)
        {
          return (_context.Games?.Any(e => e.GameId == id)).GetValueOrDefault();
        }
    }
}

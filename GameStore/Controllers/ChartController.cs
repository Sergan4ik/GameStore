using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly GamesDBContext _context;

        public ChartController(GamesDBContext context)
        {
            _context = context;
        }
        
        [HttpGet("Games")]
        public JsonResult Games()
        {
            var games = _context.Games.Include(g => g.GameCopies).ToList();
            List<object> gamesCopy = new List<object>()
            {
                new[] { "Гра", "К-сть проданих копій" }
            };
            games.ForEach(g => gamesCopy.Add(new object[]{g.Name , g.GameCopies.Count}));
            return new JsonResult(gamesCopy);
        }

        [HttpGet("GameStudiosCapitalization")] // sold copies * price + items * price
        public JsonResult Capitalization()
        {
            var gameStudios = _context.GameStudios
                .Include(s => s.Games)
                .ThenInclude(g => g.Items)
                .ThenInclude(i => i.ItemsInInventories)
                .Include(g => g.Games)
                .ThenInclude(g => g.GameCopies);
            List<object> capitalizations = new List<object>()
            {
                new[] { "Game", "Studio", "Capitalization"}
            };

            decimal? marketCap = 0;
            foreach (var gs in gameStudios)
            {
                decimal? capitalization = 0;
                string studioName = gs.StudioName ?? "";
                foreach (var game in gs.Games)
                {
                    decimal? localCap = 0;
                    localCap += game.Price * game.GameCopies.Count;
                    foreach (var item in game.Items)
                    {
                        localCap += item.ItemsInInventories.Count * item.Price;
                    }
                    
                    capitalizations.Add(new object[]
                    {
                        game.Name, studioName , localCap == null ? "" : localCap.Value
                    });
                    capitalization += localCap;
                }
                capitalizations.Add(new object[]
                {
                    studioName, "Капіталізація ринку" , capitalization == null ? "" : capitalization.Value
                });
                marketCap += capitalization;
            }
            
            capitalizations.Add(new object[]
            {
                "Капіталізація ринку", null , marketCap.Value
            });

            return new JsonResult(capitalizations);
        }
        
        
    }
}

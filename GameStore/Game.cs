using System;
using System.Collections.Generic;

namespace GameStore
{
    public partial class Game
    {
        public Game()
        {
            GameCopies = new HashSet<GameCopy>();
            Items = new HashSet<Item>();
            Promocodes = new HashSet<Promocode>();
        }

        public int GameId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? GameStudioId { get; set; }
        public int? AgePermission { get; set; }
        public decimal? Price { get; set; }
        public string? Genre { get; set; }

        public virtual GameStudio? GameStudio { get; set; }
        public virtual ICollection<GameCopy> GameCopies { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Promocode> Promocodes { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Назва гри")]
        public string Name { get; set; } = null!;
        [Display(Name = "Опис")]
        public string? Description { get; set; }
        public int? GameStudioId { get; set; }
        [Display(Name = "Вікові обмеження")]
        public int? AgePermission { get; set; }
        [Display(Name = "Ціна")]
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }
        [Display(Name = "Жанр")]
        public string? Genre { get; set; }

        public virtual GameStudio? GameStudio { get; set; }
        public virtual ICollection<GameCopy> GameCopies { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Promocode> Promocodes { get; set; }
    }
}

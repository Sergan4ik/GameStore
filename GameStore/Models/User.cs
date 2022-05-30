using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameStore
{
    public partial class User
    {
        public User()
        {
            GameCopies = new HashSet<GameCopy>();
            ItemsInInventories = new HashSet<ItemsInInventory>();
        }

        public int Id { get; set; }
        [Display(Name ="Нікнейм")]
        public string Username { get; set; } = null!;
        [Display(Name ="Емейл")]
        public string? Email { get; set; }
        [Display(Name = "Дата народження")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        [Display(Name = "Баланс")]
        public decimal? Balance { get; set; }

        public virtual ICollection<GameCopy> GameCopies { get; set; }
        public virtual ICollection<ItemsInInventory> ItemsInInventories { get; set; }
    }
}

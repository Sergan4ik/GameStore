using System;
using System.Collections.Generic;

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
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public decimal? Balance { get; set; }

        public virtual ICollection<GameCopy> GameCopies { get; set; }
        public virtual ICollection<ItemsInInventory> ItemsInInventories { get; set; }
    }
}

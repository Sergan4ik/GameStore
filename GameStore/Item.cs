using System;
using System.Collections.Generic;

namespace GameStore
{
    public partial class Item
    {
        public Item()
        {
            ItemsInInventories = new HashSet<ItemsInInventory>();
        }

        public int Id { get; set; }
        public int? Game { get; set; }
        public decimal? Price { get; set; }
        public string? Rarity { get; set; }

        public virtual Game? GameNavigation { get; set; }
        public virtual ICollection<ItemsInInventory> ItemsInInventories { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameStore
{
    public partial class Item
    {
        public Item()
        {
            ItemsInInventories = new HashSet<ItemsInInventory>();
        }

        public int Id { get; set; }
        [Display(Name = "Назва")]
        public string Name { get; set; } = null!;
        [Display(Name = "Гра")]
        public int? Game { get; set; }
        [Display(Name = "Ціна")]
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }
        [Display(Name = "Рідкість")]
        public string? Rarity { get; set; }

        public virtual Game? GameNavigation { get; set; }
        public virtual ICollection<ItemsInInventory> ItemsInInventories { get; set; }
    }
}

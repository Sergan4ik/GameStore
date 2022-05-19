using System;
using System.Collections.Generic;

namespace GameStore
{
    public partial class ItemsInInventory
    {
        public int ItemCopyId { get; set; }
        public int? SourceItemId { get; set; }
        public int? OwnerId { get; set; }

        public virtual User? Owner { get; set; }
        public virtual Item? SourceItem { get; set; }
    }
}

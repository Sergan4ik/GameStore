using System;
using System.Collections.Generic;

namespace GameStore
{
    public partial class Promocode
    {
        public string Promocode1 { get; set; } = null!;
        public int? Game { get; set; }
        public double? Discount { get; set; }

        public virtual Game? GameNavigation { get; set; }
    }
}

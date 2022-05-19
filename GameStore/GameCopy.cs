using System;
using System.Collections.Generic;

namespace GameStore
{
    public partial class GameCopy
    {
        public int CopyId { get; set; }
        public int? UserId { get; set; }
        public int? GameId { get; set; }
        public DateTime? BuyDate { get; set; }

        public virtual Game? Game { get; set; }
        public virtual User? User { get; set; }
    }
}

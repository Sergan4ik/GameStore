using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameStore
{
    public partial class GameCopy
    {
        public int CopyId { get; set; }
        public int? UserId { get; set; }
        public int? GameId { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Дата покупки")]
        public DateTime? BuyDate { get; set; }

        public virtual Game? Game { get; set; }
        public virtual User? User { get; set; }
    }
}

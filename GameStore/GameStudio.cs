using System;
using System.Collections.Generic;

namespace GameStore
{
    public partial class GameStudio
    {
        public GameStudio()
        {
            Games = new HashSet<Game>();
        }

        public int Id { get; set; }
        public string? StudioName { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Game> Games { get; set; }
    }
}

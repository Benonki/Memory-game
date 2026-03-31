using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_game_shared.Models
{
    public class Card
    {
        public int id { get; set; }
        public int pairId { get; set; }
        public string imagePath { get; set; } = string.Empty;
        public bool isFaceUp { get; set; } = false;
        public bool isMatched { get; set; }
    }
}

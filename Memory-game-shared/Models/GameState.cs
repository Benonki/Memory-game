using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_game_shared.Models
{
    public class GameState
    {
        public List<Card> Cards { get; set; } = new List<Card>();
        public string CurrentPlayerId { get; set; }
        public Dictionary<string, int> Scores { get; set; } = new Dictionary<string, int>();
        public bool IsGameOver { get; set; }

    }
}

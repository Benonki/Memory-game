using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_game_shared.Models
{
    public class GameSettings
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public string[] ImagePaths { get; set; }
        public string DeckName { get; set; } = string.Empty;
        public byte[] DeckZipData { get; set; } = Array.Empty<byte>();
        public string LobbyName { get; set; } = "Memory Game Lobby";
        public int TurnTimeSeconds { get; set; } = 5;
    }
}

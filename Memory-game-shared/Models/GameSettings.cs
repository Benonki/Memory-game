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
    }
}

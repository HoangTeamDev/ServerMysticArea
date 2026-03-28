using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.Player
{
    public class PlayerDeck
    {
        public int DeckId { get; set; }
        public string DeckName { get; set; } = string.Empty;
        public bool isActive;
        public String formatType = string.Empty; 
        public Dictionary<int, int> Cards { get; set; } = new Dictionary<int, int>();
    }
}

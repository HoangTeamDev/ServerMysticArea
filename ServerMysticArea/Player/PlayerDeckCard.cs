using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.Player
{
    public class PlayerDeckCard
    {
            public int DeckCardId { get; set; }
            public int DeckId { get; set; }
            public Dictionary<int, int> Cards { get; set; } = new Dictionary<int, int>();
    }
}

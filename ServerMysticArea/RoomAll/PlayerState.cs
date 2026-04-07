using ServerMysticArea.CardData;
using ServerMysticArea.GameServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.RoomAll
{
    public class PlayerState
    {
        public PlayerSession Session { get;  set; }
        public int HP;
        public Dictionary<int, Card> Deck = new Dictionary<int, Card>();
        public List<Card> Hand = new List<Card>();
        public List<Card> Field = new List<Card>();
        public List<Card> Graveyard = new List<Card>();
    }
}

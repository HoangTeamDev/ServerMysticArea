using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.Player
{
    public class PlayerData
    {
        public int PlayerId;
        public int AccountId;

        public string Nickname = string.Empty;
        public int Level;
        public long Exp;
        public long Gold;
        public long Diamond;
        public PlayerCard playerCard=new PlayerCard();
        public List<PlayerDeck> playerDecks= new List<PlayerDeck>();
        public PlayerDeckCard playerDeckCard=new PlayerDeckCard();

    }
}

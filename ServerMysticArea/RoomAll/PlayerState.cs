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
       
        public List<CardInstance> Hand = new List<CardInstance>();
        public List<CardInstance> Deck = new List<CardInstance>();
        public List<CardInstance> Graveyard = new List<CardInstance>();
        public CardInstance[] Monsterzone= new CardInstance[5];
        public CardInstance[] Trapzone= new CardInstance[5];
        public bool HasNormal { get; set; }
        public bool isDrawStart { get; set; }
    }
}

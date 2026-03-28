using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.CardData
{
    public class Card
    {
        public int _CardId;
        public string _Name = string.Empty;
        public int _Attack;
        public int _Hp;
        public int _CardType;
        public int _Level;
        public string _Rarity = string.Empty;
        public int _Cost;
        public int _Race;
        public int _Element;
        public int _KeyWord;
        public List<CardEffects> CardEffects = new List<CardEffects>();

    }
}

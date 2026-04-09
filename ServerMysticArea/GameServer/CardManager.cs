using ServerMysticArea.CardData;
using ServerMysticArea.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.GameServer
{

    public static class CardManager
    {
        public static Dictionary<int, Card> Cards = new();

        public static void LoadAll()
        {
            var repo = new CardRepository();
            var list = repo.LoadAllCards();
            Cards = list;


            Console.WriteLine($"Loaded {Cards.Count} cards!");
        }

        public static Card Get(int id)
        {
            return Cards[id];
        }
        public static byte Getcardtype(int id)
        {
            return (byte)Cards[id]._CardType;
        }
    }
}

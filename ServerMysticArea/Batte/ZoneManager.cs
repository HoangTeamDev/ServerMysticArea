using ServerMysticArea.RoomAll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.Batte
{
    public class ZoneManager
    {
        public CardInstance DrawCard(PlayerState player)
        {
            if (player == null) return null;
            if (player.Deck == null || player.Deck.Count == 0) return null;

            var card = player.Deck[0];
            player.Deck.RemoveAt(0);
            player.Hand.Add(card);

            card.CurrentZone = ZoneType.Hand;
            card.SlotIndex = -1;
            card.IsFaceUp = true;

            return card;
        }
        public List<CardInstance> DrawMulti(PlayerState player, int quantity)
        {
            List<CardInstance> cardInstances = new List<CardInstance>();
            if (player == null) return null;
            if (player.Deck == null || player.Deck.Count == 0) return null;
            for (int i = 0; i < quantity; i++)
            {
                var card = player.Deck[0];
                player.Deck.RemoveAt(0);
                player.Hand.Add(card);

                card.CurrentZone = ZoneType.Hand;
                card.SlotIndex = -1;
                card.IsFaceUp = true;
                cardInstances.Add(card);
            }
            return cardInstances;
        }
    }
}

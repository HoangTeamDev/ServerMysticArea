using ServerMysticArea.GameServer;
using ServerMysticArea.RoomAll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.Batte
{
    public class CardInstanceManager
    {
        private long _nextInstanceId = 0;

        public long GetNextInstanceId()
        {
            return Interlocked.Increment(ref _nextInstanceId);
        }

        public CardInstance CreateCardInstance(int cardId, int ownerPlayerId)
        {
            return new CardInstance
            {
                InstanceId = GetNextInstanceId(),
                CardId = cardId,
                OwnerPlayerId = ownerPlayerId,
                ControllerPlayerId = ownerPlayerId,
                CurrentZone = ZoneType.Deck,
                SlotIndex = -1,
                IsFaceUp = false
            };
        }

        public List<CardInstance> CreateDeck(PlayerSession playerSession)
        {
            List<CardInstance> deck = new List<CardInstance>();

            foreach (var data in playerSession.PlayerData.playerDeckCard.Cards)
            {
                int cardId = data.Key;
                int count = data.Value;

                while (count > 0)
                {
                    deck.Add(CreateCardInstance(cardId, playerSession.PlayerId));
                    count--;
                }
            }

            Shuffle(deck);
            return deck;
        }

        private void Shuffle(List<CardInstance> deck)
        {
            Random rng = new Random();

            for (int i = deck.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (deck[i], deck[j]) = (deck[j], deck[i]);
            }
        }
    }
}

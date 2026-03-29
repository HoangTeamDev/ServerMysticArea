using ServerMysticArea.CardData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public PlayerCard playerCard = new PlayerCard();
        public List<PlayerDeck> playerDecks = new List<PlayerDeck>();
        public PlayerDeckCard playerDeckCard = new PlayerDeckCard();
        public void AddCard(int cardId)
        {
            if (playerDeckCard.Cards.ContainsKey(cardId))
            {
                if (playerDeckCard.Cards.TryGetValue(cardId, out int value))
                {
                    if (value >= 3)
                    {
                        Console.WriteLine("Đã đạt số lượng tối đa của thẻ này trong bộ bài");
                        return;
                    }
                    else
                    {
                        playerDeckCard.Cards[cardId] = value + 1;
                        playerCard.AllCard[cardId] -= 1;
                        if (playerCard.AllCard[cardId] == 0)
                        {
                            playerCard.AllCard.Remove(cardId);
                        }

                    }
                }

            }
            else
            {
                playerDeckCard.Cards.Add(cardId, 1);
                playerCard.AllCard[cardId] -= 1;
                if (playerCard.AllCard[cardId] == 0)
                {
                    playerCard.AllCard.Remove(cardId);
                }
            }

        }
        public void RemoveCard(int cardId)
        {
            if (playerDeckCard.Cards.ContainsKey(cardId))
            {
                if (playerDeckCard.Cards.TryGetValue(cardId, out int value))
                {
                    playerDeckCard.Cards[cardId] = value - 1;
                    if (playerCard.AllCard.ContainsKey(cardId))
                    {
                        playerCard.AllCard[cardId] += 1;
                    }
                    else
                    {
                        playerCard.AllCard.Add(cardId, 1);
                    }
                    if (playerDeckCard.Cards[cardId] == 0)
                    {
                        playerDeckCard.Cards.Remove(cardId);
                    }
                }
                else
                {
                    Console.WriteLine("Key không tồn tại");
                }
            }
        }

       
    }
}

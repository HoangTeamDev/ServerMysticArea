using ServerMysticArea.CardData;
using ServerMysticArea.DB;
using ServerMysticArea.Player;
using ServerMysticArea.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ServerMysticArea.GameServer
{
    public static class GameSender
    {
        public static void SendUpdatePlayerCard(PlayerSession session, int cardid)
        {
            try
            {
                int quantity = 0;
                if (session.PlayerData.playerCard.AllCard.ContainsKey(cardid))
                {
                    quantity = session.PlayerData.playerCard.AllCard[cardid];
                }
                Message message = new Message(9);
                message.writeInt(cardid);
                message.writeInt(quantity);
                session.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void SendUpdatePlayerDeck(PlayerSession session, int cardId)
        {
            try
            {
                int quantity = 0;
                if (session.PlayerData.playerDeckCard.Cards.ContainsKey(cardId))
                {
                    quantity = session.PlayerData.playerDeckCard.Cards[cardId];
                }
                Message message = new Message(11);
                message.writeInt(cardId);
                message.writeInt(quantity);
                session.Send(message);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SendAllCard(PlayerSession session, Dictionary<int, Card> cards)
        {
            try
            {
                if (session == null || cards == null || cards.Count == 0)
                    return;

                const int batchSize = 50;

               
                int totalCards = cards.Count;
                int totalBatches = (totalCards + batchSize - 1) / batchSize;

                Console.WriteLine($"Sending {totalCards} cards to client in {totalBatches} batches...");

                for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
                {
                    int start = batchIndex * batchSize;
                    int count = Math.Min(batchSize, totalCards - start);

                    Message message = new Message(5);

                    // Header của batch
                    message.writeInt(totalCards);     // tổng số card toàn bộ
                    message.writeInt(totalBatches);   // tổng số batch
                    message.writeInt(batchIndex);     // batch hiện tại, bắt đầu từ 0
                    message.writeInt(count);          // số card trong batch này

                    Console.WriteLine($"Sending batch {batchIndex + 1}/{totalBatches}, cards: {count}");

                    for (int i = 1; i <= count; i++)
                    {
                        Card card = cards[start + i];
                       

                        message.writeInt(card._CardId);
                        message.writeUTF(card._Name ?? string.Empty);
                        message.writeInt(card._Attack);
                        message.writeInt(card._Hp);
                        message.writeInt(card._CardType);
                        message.writeInt(card._Level);
                        message.writeUTF(card._Rarity ?? string.Empty);
                        message.writeInt(card._Race);
                        message.writeInt(card._Element);
                        message.writeInt(card._KeyWord);

                        int effectCount = card.CardEffects?.Count ?? 0;
                        if (effectCount > byte.MaxValue)
                            throw new Exception($"Card {card._CardId} has too many effects: {effectCount}");

                        message.writeByte((byte)effectCount);

                        if (effectCount > 0)
                        {
                            foreach (var effect in card.CardEffects)
                            {
                                message.writeInt(effect._id);
                                message.writeUTF(effect._Skillname ?? string.Empty);
                                message.writeUTF(effect._Des ?? string.Empty);
                                message.writeUTF(effect._TriggerType ?? string.Empty);
                                message.writeBool(effect._OnePerTurn);
                                message.writeUTF(effect._ActiveZone ?? string.Empty);
                                message.writeUTF(effect._triggerMode ?? string.Empty);
                            }
                        }
                    }

                    session.Send(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("SendAllCard Error: " + e.Message);
            }
        }
        public static void SendLoginFail(PlayerSession session)
        {
            try
            {
                Message message = new Message(1);
                message.writeBool(false);
                session.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void SendPlayerCard(PlayerSession session)
        {
            try
            {
                Message message = new Message(6);
                message.writeInt(session.PlayerData.playerCard.AllCard.Count);
                var playerCard = session.PlayerData.playerCard.AllCard;
                foreach (var data in playerCard)
                {
                    message.writeInt(data.Key);
                    message.writeInt(data.Value);
                }
                session.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void SendPlayerDeck(PlayerSession session)
        {
            try
            {
                Message message = new Message(7);
                message.writeInt(session.PlayerData.playerDecks.Count);

                var playerDeck = session.PlayerData.playerDecks;
                foreach (var data in playerDeck)
                {
                    message.writeInt(data.DeckId);
                    message.writeUTF(data.DeckName);
                    message.writeUTF(data.formatType);
                    message.writeBool(data.isActive);
                    message.writeInt(data.Cards.Count);
                    foreach (var data2 in data.Cards)
                    {
                        message.writeInt(data2.Key);
                        message.writeInt(data2.Value);
                    }
                }
                session.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void SendPlayerDeckCard(PlayerSession session)
        {
            try
            {
                Message message = new Message(8);
                message.writeInt(session.PlayerData.playerDeckCard.DeckCardId);
                var allcard = session.PlayerData.playerDeckCard.Cards;
                message.writeInt(allcard.Count);
                foreach (var data in allcard)
                {
                    message.writeInt(data.Key);
                    message.writeInt(data.Value);
                }
                session.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void SendLoginSucces(PlayerSession session)
        {
            try
            {
                Message message = new Message(1);
                message.writeBool(true);
                session.Send(message);
                SendInfo(session);
                SendAllCard(session, CardManager.Cards);
                var repo = new CardRepository();
                session.PlayerData.playerCard.AllCard = repo.LoadPlayerCardById(session.PlayerData.PlayerId);
               // SendPlayerCard(session);
                session.PlayerData.playerDecks = repo.LoadDecksByPlayerId(session.PlayerData.PlayerId);
               // SendPlayerDeck(session);
                foreach (var data in session.PlayerData.playerDecks)
                {
                    if (data.isActive)
                    {
                        session.PlayerData.playerDeckCard = repo.LoadDeckCards(session.PlayerData.PlayerId);
                       // SendPlayerDeckCard(session);
                        break;
                    }
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SendCreateTK(PlayerSession session)
        {
            try
            {
                Message message = new Message(1);
                message.writeBool(true);
                session.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void SendCreatePlayer(PlayerSession session)
        {
            try
            {
                Message message = new Message(3);
                message.writeBool(true);
                session.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SendNotiPlayer(PlayerSession session, string text)
        {
            try
            {
                Message message = new Message(10);
                message.writeUTF(text);
                session.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void SendInfo(PlayerSession session)
        {
            try
            {
                PlayerData playerData = session.PlayerData;
                Message message = new Message(4);
                message.writeInt(playerData.PlayerId);
                message.writeUTF(playerData.Nickname);
                message.writeInt((int)playerData.Level);
                message.writeInt((int)playerData.Gold);
                message.writeInt((int)playerData.Diamond);
                session.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

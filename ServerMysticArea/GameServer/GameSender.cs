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
        public static void SendAllCard(PlayerSession session, Dictionary<int, Card> cards)
        {
            try
            {
                Message message = new Message(5);

                message.writeInt(cards.Count);
                foreach (var data in cards)
                {
                    message.writeInt(data.Key);
                    message.writeUTF(data.Value._Name);
                    message.writeInt(data.Value._Attack);
                    message.writeInt(data.Value._Hp);
                    message.writeInt(data.Value._CardType);
                    message.writeInt(data.Value._Level);
                    message.writeUTF(data.Value._Rarity);
                    message.writeInt(data.Value._Race);
                    message.writeInt(data.Value._Element);
                    message.writeInt(data.Value._KeyWord);
                    message.writeByte((byte)data.Value.CardEffects.Count);
                    foreach (var data2 in data.Value.CardEffects)
                    {
                        message.writeInt(data2._id);
                        message.writeUTF(data2._Skillname);
                        message.writeUTF(data2._Des);
                        message.writeUTF(data2._TriggerType);
                        message.writeBool(data2._OnePerTurn);
                        message.writeUTF(data2._ActiveZone);
                        message.writeUTF(data2._triggerMode);
                    }
                }
                session.Send(message);
            }
            catch (Exception e)
            {

            }
            finally
            {

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
                SendPlayerCard(session);
                session.PlayerData.playerDecks = repo.LoadDecksByPlayerId(session.PlayerData.PlayerId);
                SendPlayerDeck(session);
                Console.WriteLine($"Player {session.PlayerData.playerDecks[0].Cards.Count} logged in successfully");
                session.PlayerData.playerDeckCard = repo.LoadDeckCards(session.PlayerData.PlayerId);
                SendPlayerDeckCard(session);
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

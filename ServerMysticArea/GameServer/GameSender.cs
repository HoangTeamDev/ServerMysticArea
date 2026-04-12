using ServerMysticArea.CardData;
using ServerMysticArea.DB;
using ServerMysticArea.Player;
using ServerMysticArea.RoomAll;
using ServerMysticArea.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ServerMysticArea.GameServer
{
    public static class GameSender
    {
        public static void SendStartGame(PlayerSession session, Room room)
        {
            try
            {
                Message message = new Message(13);
                message.writeByte(5);
                message.writeBool(room.HostPlayer.Session == session);

                message.writeInt(room.HostPlayer.HP);
                message.writeInt(room.GuestPlayer.HP);
                int countdeckHost=room.HostPlayer.Deck.Count;
                message.writeByte((byte)countdeckHost);
                foreach (var item in room.HostPlayer.Deck)
                {
                    message.writeInt((int)item.InstanceId);
                    message.writeInt(item.CardId);
                }
                message.writeByte((byte)room.GuestPlayer.Deck.Count);
                foreach (var item in room.GuestPlayer.Deck)
                {
                    message.writeInt((int)item.InstanceId);
                    message.writeInt(item.CardId);
                }
                session.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void SendCreateRoom(PlayerSession session, int roomId)
        {
            try
            {
                Message message = new Message(13);
                message.writeByte(1);
                message.writeInt(roomId);
                session.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void SendJoinRoom(PlayerSession session, PlayerSession playerSession)
        {
            try
            {
                Message message = new Message(13);
                message.writeByte(2);
                message.writeInt(playerSession.PlayerData.PlayerId);
                message.writeUTF(playerSession.PlayerData.Nickname);
                message.writeInt(playerSession.PlayerData.Level);
                session.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void SendInfoJoinRoom(PlayerSession session,PlayerSession playerSession, int zoomid)
        {
            try
            {
                Message message = new Message(13);
                message.writeByte(4);
                message.writeInt(zoomid);
                message.writeInt(playerSession.PlayerData.PlayerId);
                message.writeUTF(playerSession.PlayerData.Nickname);
                message.writeInt(playerSession.PlayerData.Level);
                session.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
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
                message.writeByte((byte)CardManager.Getcardtype(cardid));
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
                message.writeByte((byte)CardManager.Getcardtype(cardId));
                session.Send(message);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void SendEffCard(PlayerSession session, int cardId)
        {
            try
            {
                if (!CardManager.Cards.ContainsKey(cardId))
                    return;
                Card card = CardManager.Cards[cardId];
                Message message = new Message(12);
                int effectCount = card.CardEffects?.Count ?? 0;
                if (effectCount > byte.MaxValue)
                    throw new Exception($"Card {card._CardId} has too many effects: {effectCount}");

                message.writeInt(card._CardId);
                message.writeUTF(card._Name ?? string.Empty);
                message.writeInt(card._CardType);
                message.writeUTF(card._Rarity ?? string.Empty);
                message.writeInt(card._Race);
                message.writeInt(card._Hp);
                message.writeInt(card._Attack);
                message.writeInt(card._Level);
                message.writeInt(card._KeyWord);
                message.writeInt(card._Element);

                if (effectCount > 0)
                {
                    message.writeByte((byte)effectCount);

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
                Message message = new Message(5);
                message.writeInt(cards.Count);
                foreach (var card in cards.Values)
                {
                    message.writeInt(card._CardId);
                    message.writeUTF(card._Name ?? string.Empty);
                    message.writeUTF(card._Rarity ?? string.Empty);
                    message.writeByte((byte)card._KeyWord);
                    message.writeByte((byte)card._CardType);
                    if(card._CardType is 1)
                    {
                        message.writeShort((short)card._Hp);
                        message.writeShort((short)card._Attack);
                        message.writeByte((byte)card._Element);
                        message.writeByte((byte)card._Level);                       
                        message.writeByte((byte)card._Race);

                    }

                }
                session.Send(message);
                var repo = new CardRepository();
                session.PlayerData.playerCard.AllCard = repo.LoadPlayerCardById(session.PlayerData.PlayerId);
                SendPlayerCard(session);
                session.PlayerData.playerDecks = repo.LoadDecksByPlayerId(session.PlayerData.PlayerId);
                SendPlayerDeck(session);
                
                foreach (var data in session.PlayerData.playerDecks)
                {
                    Console.WriteLine("so deck" + data.isActive);
                    if (data.isActive)
                    {
                        session.PlayerData.playerDeckCard = repo.LoadDeckCards(data.DeckId);
                        SendPlayerDeckCard(session);
                        break;
                    }
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
                    message.writeByte(CardManager.Getcardtype(data.Key));
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
                    message.writeByte((byte)CardManager.Getcardtype(data.Key));
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

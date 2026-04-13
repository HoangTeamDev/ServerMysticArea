using ServerMysticArea.RoomAll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.GameServer
{
    public static class GameSenderBattle
    {
        public static void SendDrawStat(PlayerSession playerSession, PlayerState playerState)
        {
            Message message = new Message(14);
            try
            {
                message.writeByte(1);
                message.writeInt(playerState.Session.PlayerId);
                message.writeByte((byte)playerState.Hand.Count);
                foreach(var data in playerState.Hand)
                {
                    message.writeInt((int)data.InstanceId);
                    message.writeInt(data.CardId);
                   
                }
                playerSession.Send(message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public static void SendAllRoom(Room room, Message message)
        {
            room.HostPlayer.Session.Send(message);
            room.GuestPlayer.Session.Send(message);
        }

        public static Message SenDrawCard(List<CardInstance> cardInstances, int playerid)
        {
            var message = new Message(14);
            message.writeByte(2);
            message.writeInt(playerid);
            message.writeByte((byte)cardInstances.Count);
            foreach (var cardInstance in cardInstances)
            {
                message.writeInt((int)cardInstance.InstanceId);
                message.writeInt(cardInstance.CardId);
            }
            return message;
        }

        public static Message SendTitlePhase(string text)
        {
            var mess = new Message(15);
            mess.writeByte(1);
            mess.writeUTF(text);
            return mess;
        }
      
    }
}

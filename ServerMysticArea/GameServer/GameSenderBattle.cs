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
      
    }
}

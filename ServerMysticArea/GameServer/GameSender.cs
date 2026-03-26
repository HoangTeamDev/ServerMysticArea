using ServerMysticArea.Player;
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
        public static void SendLoginSucces(PlayerSession session)
        {
            try
            {
                Message message = new Message(1);
                message.writeBool(true);
                session.Send(message);
                SendInfo(session);
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
                PlayerData playerData=session.PlayerData;
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

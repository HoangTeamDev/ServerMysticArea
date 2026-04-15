using ServerMysticArea.RoomAll;
using ServerMysticArea.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.GameServer
{
    public class GameRouterBattle
    {
        public async void Handle(PlayerSession session, Message message)
        {
            Console.WriteLine($"Session:{session.SessionId}   {message.Command}");
            switch (message.Command)
            {
                case 14:
                    {
                        HandleCardBattle(session, message);
                    }
                    break;
                case 15:
                    {
                        HandlePhase(session, message);
                    }
                    break;

            }
        }
        void HandlePhase(PlayerSession session, Message message)
        {
            byte type=message.readByte();
            switch (type)
            {
                case 2:
                    {
                        Room room = MainServer._roomManager.GetRoomById(session.CurrentRoom.RoomId);
                        if (room!= null)
                        {
                            room.EndTurn(session);
                        }
                    }
                    break;
            }
        }


        void HandleCardBattle(PlayerSession session, Message message)
        {
            byte type=message.readByte();
            switch (type)
            {
                case 1:
                    {
                        bool ok=message.readBool();
                        Room room = MainServer._roomManager.GetRoomById(session.CurrentRoom.RoomId);
                        PlayerState playerState = room.GetState(session);
                        if (playerState != null)
                        {
                            playerState.isDrawStart = ok;
                        }
                    }
                    break;
                case 3:
                    {
                        int intanceid = message.readInt();
                        
                        MainServer._zoneManager.NormalSummon(session, intanceid);
                    }
                    break;
            }
        }
    }
}

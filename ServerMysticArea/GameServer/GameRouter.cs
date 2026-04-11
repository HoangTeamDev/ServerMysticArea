using ServerMysticArea.CardData;
using ServerMysticArea.DB;
using ServerMysticArea.Player;
using ServerMysticArea.RoomAll;
using ServerMysticArea.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.GameServer
{
    public  class GameRouter
    {
        public async void Handle(PlayerSession session, Message message)
        {
            Console.WriteLine($"Session:{session.SessionId}   {message.Command}");
            switch (message.Command)
            {
                case 1:
                    HandleLogin(session, message);
                    break;

                case 2:
                    HandleCreateTK(session, message);
                    break;
                 case 3:
                    HandleCreatePlayer(session, message);
                    break;
                case 9:
                    HandleAddCard(session, message);
                    break;
                case 12:
                    HandleEffCard(session, message);
                    break;
                case 13:
                    HandleCreateRoom(session, message);
                    break;

            }
        }
        void HandleCreateRoom(PlayerSession session, Message message)
        {
            int type = message.readByte();
            switch(type)
            {
                case 1:
                    MainServer._roomManager.CreateRoom(session);
                    break;
                case 2:
                    int roomId = message.readInt();
                    Room room= MainServer._roomManager.GetRoomById(roomId);
                    room.AddGuestPlayer(session);
                    PlayerSession playerSession = room.GetOpponent(session);
                   
                    Console.WriteLine($"Người chơi {session.PlayerData.Nickname} đã tham gia phòng {room.RoomId} của {playerSession.PlayerData.Nickname}");
                    GameSender.SendJoinRoom(room.GetOpponent(session),session);
                    GameSender.SendInfoJoinRoom(session, playerSession, roomId);
                    /* if (room == null)
                     {
                         GameSender.SendNotiPlayer(session, "Phòng không tồn tại!!!");
                         return;
                     }
                     if(room.HostPlayer.Session == session)
                     {
                         GameSender.SendNotiPlayer(session, "Bạn đã là chủ phòng này rồi!!!");
                         return;
                      }
                     if(room.GuestPlayer != null)
                     {
                         GameSender.SendNotiPlayer(session, "Phòng đã đủ người chơi!!!");
                         return;
                     }
                     if(session.CurrentRoom != null)
                     {
                         GameSender.SendNotiPlayer(session, "Bạn đã ở trong phòng khác rồi!!!");
                         return;
                     }
                     else
                     {

                     }*/



                    break;
                case 3:
                    MainServer._roomManager.RemovePlayerFromRoom(session);
                    break;
                case 5:
                    {
                        Room room1 = MainServer._roomManager.GetRoomById(session.CurrentRoom.RoomId);
                        room1.StartGame();
                    }
                    break;
            }
           
           
           
        }
        void HandleEffCard(PlayerSession session, Message message)
        {  
            int cardId = message.readInt();
           
           

            GameSender.SendEffCard(session,cardId);
               
                Console.WriteLine($"Gửi thông tin thẻ cho người chơi {session.PlayerData.Nickname}");

        }
        void HandleAddCard(PlayerSession session, Message message)
        {
            int type = message.readByte();
            int cardId = message.readInt();
            switch (type)
            {
                case 1:
                    session.PlayerData.AddCard(cardId);
                    break;
                case 2:
                    session.PlayerData.RemoveCard(cardId);
                    break;
            }
            GameSender.SendUpdatePlayerCard(session,cardId);
            GameSender.SendUpdatePlayerDeck(session,cardId);
            Console.WriteLine($"Thêm thẻ {cardId} cho người chơi {session.PlayerData.Nickname}");
           
        }
        async void HandleCreatePlayer(PlayerSession session, Message message)
        {
            string playerName = message.readUTF();
            PlayerData playerData1= await PlayerRepository.GetByAccountId(session.accountData.AccountId);
            if(playerData1 == null)
            {
                PlayerData playerData = new PlayerData();
                playerData.Nickname = playerName;
                playerData.PlayerId = RandomNumberGenerator.GetInt32(5, 1000);
                playerData.AccountId = session.accountData.AccountId;
                playerData.Level = 1;
                playerData.Gold = 0;
                playerData.Exp = 0;
                session.SetPlayerData(playerData);
                await PlayerRepository.InsertPlayerAsync(session.PlayerData);
                GameSender.SendNotiPlayer(session, "Bạn đã tạo nhân vật thành công!!!");
                GameSender.SendLoginSucces(session);
            }
            else
            {
                GameSender.SendNotiPlayer(session, $"Tên nhân vật {playerName} đã tồn tại!!!");
            }
            
        }
        async void HandleCreateTK(PlayerSession session, Message message)
        {
            string playerName = message.readUTF();
            string passwprd = message.readUTF();
            Console.WriteLine("NHan tai khoan:" + playerName + "__" + passwprd);
            var account = await AccountRepository.GetByUsername(playerName,passwprd);
            if (account == null)
            {
                int r = RandomNumberGenerator.GetInt32(5, 1000);
                await AccountRepository.InsertAccountAsync(r, playerName, passwprd);
                GameSender.SendNotiPlayer(session, "Bạn đã tạo tài khoản thành công!!!");
            }
            else
            {
                GameSender.SendNotiPlayer(session, "Tài khoản đã tồn tại !!!");
            }
        }
        private async void HandleLogin(PlayerSession session, Message message)
        {
            string username = message.readUTF();
            string password = message.readUTF();
            // Here you would normally validate the username and password.
            Console.WriteLine($"Login attempt: {username} / {password}");
            // For this example, we'll just accept any login.


            var account = await AccountRepository.GetByUsername(username,password);
            if (account != null)
            {
                Console.WriteLine($"Acc{account.AccountId}");
                session.SetAccountData(account);
                PlayerData player = await PlayerRepository.GetByAccountId(account.AccountId);

                if (player != null)
                {
                    session.SetPlayerData(player);
                    GameSender.SendLoginSucces(session);
                    Console.WriteLine($"✅ LOGIN: {player.Nickname} (Session {session.SessionId})");
                }
                else
                {
                    GameSender.SendCreatePlayer(session);
                }
            }
            else
            {
                GameSender.SendNotiPlayer(session, "Tài khoản hoặc mật khẩu không chính xác !!!");
            }
            


        }

       
    }
}

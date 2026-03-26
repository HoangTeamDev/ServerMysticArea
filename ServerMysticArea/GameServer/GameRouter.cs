using ServerMysticArea.DB;
using ServerMysticArea.Player;
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
                
            }
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
            var account = await AccountRepository.GetByUsername(playerName);
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


            var account = await AccountRepository.GetByUsername(username);
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

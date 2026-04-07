using ServerMysticArea.CardData;
using ServerMysticArea.GameServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.RoomAll
{
    public class Room
    {
        public int RoomId { get; private set; }
        public PlayerState HostPlayer { get; private set; }
        public PlayerState GuestPlayer { get; private set; }

        public bool IsStarted { get; private set; }
        public bool IsFinished { get; private set; }

        public PlayerSession CurrentTurnPlayer { get; private set; }
        public int TurnNumber { get; private set; }

        // Ví dụ state game
       
        public DateTime TurnStartTime { get; private set; }
        public int TurnDurationSeconds { get; private set; } = 60;

        public PlayerState GetState(PlayerSession player)
        {
            if (HostPlayer.Session == player) return HostPlayer;
            if (GuestPlayer.Session == player) return GuestPlayer;
            return null;
        }
        public int GetRemainingTurnTime()
        {
            double elapsed = (DateTime.UtcNow - TurnStartTime).TotalSeconds;
            return Math.Max(0, TurnDurationSeconds - (int)elapsed);
        }
        public void Update()
        {
            if (!IsStarted || IsFinished) return;
            if (CurrentTurnPlayer == null) return;

            if (GetRemainingTurnTime() <= 0)
            {
                HandleTurnTimeout();
            }
        }
        private void HandleTurnTimeout()
        {
            EndTurn(CurrentTurnPlayer);
        }
        public void EndTurn(PlayerSession player)
        {
            if (!IsStarted || IsFinished) return;
            if (player != CurrentTurnPlayer) return;

            CurrentTurnPlayer = GetOpponent(player);
            TurnNumber++;
            TurnStartTime = DateTime.UtcNow;
        }
        public Room(int roomId, PlayerSession hostPlayer)
        {
            RoomId = roomId;
            HostPlayer = new PlayerState
            {
                Session = hostPlayer,
                HP = 8000
            };
            GuestPlayer = new PlayerState();
            IsStarted = false;
            IsFinished = false;
            TurnNumber = 0;
        }

        public bool AddGuestPlayer(PlayerSession guest)
        {
            if (guest == null) return false;
            
            if (HostPlayer.Session == guest) return false;
            if (IsStarted || IsFinished) return false;

            GuestPlayer.Session = guest;
            guest.CurrentRoom = this;
            return true;
        }

        public bool CanStart()
        {
            return HostPlayer != null && GuestPlayer != null && !IsStarted && !IsFinished;
        }

        public void StartGame()
        {
            if (!CanStart()) return;

            IsStarted = true;
            IsFinished = false;
            TurnNumber = 1;
            HostPlayer.HP = 8000;
            GuestPlayer.HP = 8000;

           
            GameSender.SendStartGame(HostPlayer.Session, this);
                GameSender.SendStartGame(GuestPlayer.Session, this);
            Console.WriteLine($"Room {RoomId} started: {HostPlayer.Session.PlayerData.Nickname} vs {GuestPlayer.Session.PlayerData.Nickname}");
        }

        public PlayerSession GetOpponent(PlayerSession player)
        {
            if (player == HostPlayer.Session) return GuestPlayer.Session;
            if (player == GuestPlayer.Session) return HostPlayer.Session;
            return null;
        }
        public void HandleDisconnect(PlayerSession player, RoomManager roomManager)
        {
            if (player == null) return;

            if (!IsStarted)
            {
                if (player == HostPlayer.Session)
                    HostPlayer = null;
                else if (player == GuestPlayer.Session)
                    GuestPlayer = null;

                player.CurrentRoom = null;

                if (HostPlayer == null && GuestPlayer == null)
                {
                    roomManager.RemoveRoom(RoomId);
                }
                else if (HostPlayer == null && GuestPlayer != null)
                {
                    HostPlayer = GuestPlayer;
                    GuestPlayer = null;
                }

                return;
            }

            var winner = GetOpponent(player);
            EndGame(winner, player, roomManager);
        }

        public void EndGame(PlayerSession winner, PlayerSession loser, RoomManager roomManager)
        {
            if (IsFinished) return;

            IsFinished = true;

            Console.WriteLine($"Room {RoomId} ended. Winner: {winner?.PlayerData.Nickname}");

            roomManager.RemoveRoom(RoomId);
        }
    }
}

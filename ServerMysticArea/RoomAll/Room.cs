using ServerMysticArea.CardData;
using ServerMysticArea.GameServer;
using ServerMysticArea.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.RoomAll
{
    public enum ZoneType
    {
        Deck,
        Hand,
        Monster,
        SpellTrap,
        Graveyard,
        Banished
    }
    public enum PhaseType
    {
        Start,
        Draw,
        Main,       
        End
    }
    
    public class Room
    {
        public int RoomId { get; private set; }
        public PlayerState HostPlayer { get;  set; }
        public PlayerState GuestPlayer { get;  set; }

        public bool IsStarted { get;  set; }
        public bool IsFinished { get;  set; }

        public PlayerState CurrentTurnPlayerId { get;  set; }
        public int TurnNumber { get;  set; }
        public PhaseType CurrentPhase { get; set; }
        // Ví dụ state game

        public DateTime TurnStartTime { get;  set; }
        public int TurnDurationSeconds { get;  set; } = 60;
        public int WinnerPlayerId;
        public bool isDrawStartFinishes =false;
        public PlayerState GetState(PlayerSession player)
        {
            if (HostPlayer.Session == player) return HostPlayer;
            if (GuestPlayer.Session == player) return GuestPlayer;
            return null;
        }
        public PlayerState GetOpState(PlayerState playerState)
        {
            if(HostPlayer==playerState)return GuestPlayer;
            if(GuestPlayer== playerState)return HostPlayer;
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
            if(HostPlayer.isDrawStart && GuestPlayer.isDrawStart)
            {
                MainServer._battleManager.StartFirstTurn(this);
                HostPlayer.isDrawStart = false;
                GuestPlayer.isDrawStart = false;
                isDrawStartFinishes = true;
            }
            
            if (isDrawStartFinishes)
            {
                if (GetRemainingTurnTime() <= 0)
                {
                    HandleTurnTimeout();
                }
            }
           
        }
        private void HandleTurnTimeout()
        {
            EndTurn(CurrentTurnPlayerId.Session);
        }
        public void EndTurn(PlayerSession player)
        {
            if (!IsStarted || IsFinished) return;
            MainServer._turnManager.EndTurn(this, player);   
            TurnNumber++;
            TurnStartTime = DateTime.UtcNow;
        }
        public Room(int roomId, PlayerSession hostPlayer)
        {
            RoomId = roomId;
            HostPlayer = new PlayerState
            {
                Session = hostPlayer,
              
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
            try
            {
                if (!CanStart()) return;
                
                MainServer._battleManager.StartBattle(this);      
                Console.WriteLine($"Room {RoomId} started: {HostPlayer.Session.PlayerData.Nickname} vs {GuestPlayer.Session.PlayerData.Nickname}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            } 
            
        }

        public PlayerSession GetOpponent(PlayerSession player)
        {
            if (player == HostPlayer.Session) return GuestPlayer.Session;
            if (player == GuestPlayer.Session) return HostPlayer.Session;
            return null;
        }
        private static long _globalInstanceId = 1;
        public List<CardInstance> CreateDeck(PlayerSession playerSession)
        {
            List<CardInstance> cardInstances = new List<CardInstance>();

            foreach (var data in playerSession.PlayerData.playerDeckCard.Cards)
            {
                int cardId = data.Key;
                int count = data.Value;

                while (count > 0)
                {
                    CardInstance card = new CardInstance
                    {
                        InstanceId = _globalInstanceId++,
                        CardId = cardId,

                        OwnerPlayerId = playerSession.PlayerId,
                        ControllerPlayerId = playerSession.PlayerId,

                        CurrentZone = ZoneType.Deck,
                        SlotIndex = -1,

                        IsFaceUp = false,

                        // nếu có stat runtime
                        CurrentAtk = 0,
                        CurrentHp = 0
                    };

                    cardInstances.Add(card);
                    count--;
                }
            }

            Shuffle(cardInstances);

            return cardInstances;
        }
        private static Random rng = new Random();

        public void Shuffle(List<CardInstance> deck)
        {
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (deck[n], deck[k]) = (deck[k], deck[n]);
            }
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

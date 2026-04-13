using ServerMysticArea.GameServer;
using ServerMysticArea.RoomAll;
using ServerMysticArea.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.Batte
{
    public class BattleManager
    {
        private  CardInstanceManager _cardInstanceManager =>MainServer._cardInstanceManager;
        private  ZoneManager _zoneManager => MainServer._zoneManager;
        private  TurnManager _turnManager => MainServer._turnManager;

       

        public void StartBattle(Room room)
        {
            InitBattleState(room);          
            room.IsStarted = true;
            room.IsFinished = false;
            MainServer._zoneManager.DrawMulti(room.HostPlayer, 5);
            MainServer._zoneManager.DrawMulti(room.GuestPlayer, 5);

            GameSenderBattle.SendDrawStat(room.HostPlayer.Session, room.HostPlayer);
            GameSenderBattle.SendDrawStat(room.HostPlayer.Session, room.GuestPlayer);
            GameSenderBattle.SendDrawStat(room.GuestPlayer.Session, room.HostPlayer);
            GameSenderBattle.SendDrawStat(room.GuestPlayer.Session, room.GuestPlayer);
            
        }
        public void StartFirstTurn(Room room)
        {
            _turnManager.StartFirstTurn(room, room.HostPlayer);
        }
        public void EndBattle(Room room, int winnerPlayerId)
        {
            if (room == null || room.HostPlayer == null || room.GuestPlayer==null)
                return;

            room.IsFinished = true;
            room.WinnerPlayerId = winnerPlayerId;
        }

        public void CheckBattleEnd(Room room)
        {
           
            if (room.HostPlayer == null || room.GuestPlayer== null|| room.IsFinished) return;

            if (room.HostPlayer.HP <= 0)
            {
                EndBattle(room, room.HostPlayer.Session.PlayerId);
                return;
            }

            if (room.GuestPlayer.HP <= 0)
            {
                EndBattle(room, room.GuestPlayer.Session.PlayerId);
                return;
            }
        }

        private void InitBattleState(Room room)
        {
            var p1Deck = _cardInstanceManager.CreateDeck(room.HostPlayer.Session);
            var p2Deck = _cardInstanceManager.CreateDeck(room.GuestPlayer.Session);
            room.HostPlayer.HP = 8000;
            room.HostPlayer.Deck = p1Deck;
            room.HostPlayer.isDrawStart=false;
            room.GuestPlayer.HP = 8000;
            room.GuestPlayer.Deck = p2Deck;
           room.GuestPlayer.isDrawStart = false;
            room.TurnNumber = 1;
            room.CurrentPhase = PhaseType.Draw;

          room.CurrentTurnPlayerId = room.HostPlayer;
            GameSender.SendStartGame(room.HostPlayer.Session, room);
            GameSender.SendStartGame(room.GuestPlayer.Session, room);
        }

        
    }
}

using ServerMysticArea.RoomAll;
using ServerMysticArea.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.Batte
{
    public class TurnManager
    {
       

        public const int TURN_TIME_SECONDS = 60;

      

        public void StartFirstTurn(Room room, PlayerState firstPlayerId)
        {
            if (room == null || room.HostPlayer == null || room.GuestPlayer == null || room.IsFinished)
                return;

            

            room.CurrentTurnPlayerId = firstPlayerId;
            room.TurnNumber = 1;

            StartTurn(room, drawCard: false);
            Console.WriteLine($"Bắt đầu trận. Player {firstPlayerId} đi trước.");
        }

        public void StartTurn(Room room, bool drawCard = true)
        {
            if (room == null || room.HostPlayer == null || room.GuestPlayer == null || room.IsFinished)
                return;

            var state = room.CurrentTurnPlayerId;
           

            

            ResetTurnFlags(state);

            room.CurrentPhase = PhaseType.Draw;
            room.TurnStartTime = DateTime.UtcNow.AddSeconds(TURN_TIME_SECONDS);

            Console.WriteLine( $"Bắt đầu lượt {room.TurnNumber} của Player {state.Session.PlayerData.Nickname}.");

            if (drawCard)
            {
                var drawnCard = MainServer._zoneManager.DrawCard(state);

                if (drawnCard != null)
                {
                    Console.WriteLine(
                        $"Player {state.Session.PlayerId} rút 1 lá. Deck còn {state.Deck.Count}, Hand có {state.Hand.Count}.");
                }
                else
                {
                    Console.WriteLine( $"Player {state.Session.PlayerId} không thể rút bài vì deck trống.");
                    HandleDeckOut(room, state);
                    return;
                }
            }

            MoveNextFromDrawPhaseIfNeeded(room);
        }

        public bool TryChangePhase(Room room, PlayerState playerId, PhaseType nextPhase, out string reason)
        {
            reason = string.Empty;

            if (room == null || room.HostPlayer == null || room.GuestPlayer== null)
            {
                reason = "Room hoặc BattleState null";
                return false;
            }

            if (room.IsFinished)
            {
                reason = "Trận đã kết thúc";
                return false;
            }

            var state = room.CurrentTurnPlayerId;

            if (state != playerId)
            {
                reason = "Không phải lượt của bạn";
                return false;
            }

            if (!IsValidNextPhase(room.CurrentPhase, nextPhase))
            {
                reason = $"Không thể chuyển từ phase {room.CurrentPhase} sang {nextPhase}";
                return false;
            }

            room.CurrentPhase = nextPhase;
            Console.WriteLine($"Player {playerId} chuyển phase sang {nextPhase}.");

            if (nextPhase == PhaseType.End)
            {
                EndTurn(room);
            }

            return true;
        }

        

        public void EndTurn(Room room)
        {
            if (room == null || room.HostPlayer == null || room.GuestPlayer ==null || room.IsFinished)
                return;

            var state = room.CurrentTurnPlayerId;
          

            Console.WriteLine($"Kết thúc lượt {room.TurnNumber} của Player {state.Session.PlayerId}.");

            ClearEndTurnTemporaryFlags(state);

            var opponent = room.GetOpState(state);
            if (opponent == null)
                return;

            room.CurrentTurnPlayerId = opponent;
            room.TurnNumber++;

            StartTurn(room, drawCard: true);
        }

     

        public bool IsCurrentPlayer(Room room, PlayerState playerId)
        {
            if (room == null || room.HostPlayer == null|| room.GuestPlayer==null)
                return false;

            return room.CurrentTurnPlayerId == playerId;
        }

        private void ResetTurnFlags(PlayerState player)
        {
            player.HasNormal = false;

            if (player.Monsterzone == null)
                return;

            for (int i = 0; i < player.Monsterzone.Length; i++)
            {
                var card = player.Monsterzone[i];
                if (card != null)
                {
                    card.HasAttackedThisTurn = false;
                }
            }
        }

        private void ClearEndTurnTemporaryFlags(PlayerState player)
        {
            if (player == null || player.Monsterzone == null)
                return;

            for (int i = 0; i < player.Monsterzone.Length; i++)
            {
                var card = player.Monsterzone[i];
                if (card != null)
                {
                    // chỗ này sau này bạn có thể clear buff/debuff tồn tại đến hết lượt
                    // ví dụ card.TempAtkBuff = 0;
                }
            }
        }

        private bool IsValidNextPhase(PhaseType current, PhaseType next)
        {
            return current switch
            {
                PhaseType.Start => next == PhaseType.Draw,
                PhaseType.Draw => next == PhaseType.Main ||next== PhaseType.End,
                PhaseType.Main =>  next == PhaseType.End,           
                PhaseType.End => false,
                _ => false
            };
        }

        private void MoveNextFromDrawPhaseIfNeeded(Room room)
        {
            if (room == null || room.HostPlayer == null|| room.GuestPlayer==null)
                return;

           

            if (room.CurrentPhase == PhaseType.Draw)
            {
                room.CurrentPhase = PhaseType.Main;
                Console.WriteLine( $"Tự động chuyển sang phase {PhaseType.Main}.");
            }
        }

        private void HandleDeckOut(Room room, PlayerState loserPlayerId)
        {
            if (room == null || room.HostPlayer == null|| room.GuestPlayer==null)
                return;

            var state = room.CurrentTurnPlayerId;
            var winner = room.GetOpState(state);

            room.IsFinished = true;
            room.WinnerPlayerId = winner?.Session.PlayerId ?? 0;

            Console.WriteLine(
                $"Player {loserPlayerId} thua do không thể rút bài. Winner = {room.WinnerPlayerId}");
        }

       
    }
}

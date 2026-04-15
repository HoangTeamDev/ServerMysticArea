using ServerMysticArea.GameServer;
using ServerMysticArea.RoomAll;
using ServerMysticArea.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.Batte
{
    public class ZoneManager
    {
        public CardInstance DrawCard(PlayerState player)
        {
            if (player == null) return null;
            if (player.Deck == null || player.Deck.Count == 0) return null;

            var card = player.Deck[0];
            player.Deck.RemoveAt(0);
            player.Hand.Add(card);

            card.CurrentZone = ZoneType.Hand;
            card.SlotIndex = -1;
            card.IsFaceUp = true;

            return card;
        }
        public List<CardInstance> DrawMulti(PlayerState player, int quantity)
        {
            List<CardInstance> cardInstances = new List<CardInstance>();
            if (player == null) return null;
            if (player.Deck == null || player.Deck.Count == 0) return null;
            for (int i = 0; i < quantity; i++)
            {
                var card = player.Deck[0];
                player.Deck.RemoveAt(0);
                player.Hand.Add(card);

                card.CurrentZone = ZoneType.Hand;
                card.SlotIndex = -1;
                card.IsFaceUp = true;
                cardInstances.Add(card);
            }
            return cardInstances;
        }

        public bool NormalSummon(PlayerSession session, int cardidd)
        {
            Room room = MainServer._roomManager.GetRoomById(session.CurrentRoom.RoomId);
            if (room == null) return false;
            PlayerState player = room.GetState(session);
            
           
            CardInstance? card= player.Hand.FirstOrDefault(x=>x.InstanceId==cardidd);
            // 1. Validate cơ bản (zone level)
            if (card == null)
            {
                Console.WriteLine("khong tim thay card tren tay");
                return false;
            }

            // phải đang ở hand
            if (card.CurrentZone != ZoneType.Hand)
                return false;
            int slot = GetZone(player);
            



            // 2. Remove khỏi zone cũ (hand)
            player.Hand.Remove(card);

            // 3. Đặt lên sân
            player.Monsterzone[slot] = card;

            // 4. Update trạng thái
            card.CurrentZone = ZoneType.Monster;
            card.SlotIndex = slot;
            card.ControllerPlayerId = player.Session.PlayerId;
            card.IsFaceUp = true;
            card.HasAttackedThisTurn = true;
            GameSenderBattle.SendAllRoom(room,GameSenderBattle.SendNomalSummon(session.PlayerId, card));
            return true;
        }

        public int GetZone(PlayerState player)
        {
            int count = player.Monsterzone.Length;
            if (count == 0) return -1;

            int mid = count / 2;

            // 1. Check center trước
            if (player.Monsterzone[mid] == null)
            {
                return mid;

            }

            // 2. Expand ra 2 bên
            for (int offset = 1; offset <= mid; offset++)
            {
                int right = mid + offset;
                if (right < count && player.Monsterzone[right] == null)
                    return right;

                int left = mid - offset;
                if (left >= 0 && player.Monsterzone[left] == null)
                    return left;
            }
            return -1;
        }
    }
}

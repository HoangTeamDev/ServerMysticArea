using Microsoft.VisualBasic;
using ServerMysticArea.CardData;
using ServerMysticArea.GameServer;
using ServerMysticArea.RoomAll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.Batte
{
    public class RuleValidator
    {
        public bool CanNormalSummon(Room room, PlayerSession playerSession ,long cardInstanceId,int slot)
        {


            var state = room.GetState(playerSession);

            // 1. Đúng lượt không
            if (room.CurrentTurnPlayerId != state)
            {
             
                return false;
            }

            // 2. Đúng phase không
            if (room.CurrentPhase != PhaseType.Main
                )
            {
                
                return false;
            }

            // 3. Đã normal summon trong lượt này chưa
            if (!state.HasNormal)
            {
              
                return false;
            }

            // 4. Slot hợp lệ không
            if (slot < 0 || slot >= state.Monsterzone.Length)
            {
               
            }

            // 5. Slot có trống không
            if (state.Monsterzone[slot] != null)
            {
              
                return false;
            }

            // 6. Lá bài có ở trên tay không
            var card = state.Hand.FirstOrDefault(x => x.InstanceId == cardInstanceId);
            if (card == null)
            {
              
                return false;
            }

            // 7. Có đúng đang ở Hand không
            if (card.CurrentZone != ZoneType.Hand)
            {
              
                return false;
            }
            Card card1 = CardManager.Get(card.CardId);
            // 8. Có phải monster không
            if (card1._CardType != 1)
            {
              
                return false;
            }

            // 9. Nếu có luật level cao cần tế phẩm thì chặn tạm
            // Bản cơ bản: chỉ cho summon quái không cần tribute
            if (card1._Level > 4)
            {
               
                return false;
            }

            return true;
        }
    }
}

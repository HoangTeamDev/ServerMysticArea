using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.RoomAll
{

    public class CardInstance
    {
        public long InstanceId { get; set; }
        public int CardId { get; set; }

        public int OwnerPlayerId { get; set; }
        public int ControllerPlayerId { get; set; }

        public ZoneType CurrentZone { get; set; }
        public int SlotIndex { get; set; }

        public bool IsFaceUp { get; set; }
        public int CurrentAtk { get; set; }
        public int CurrentHp { get; set; }
        public bool HasAttackedThisTurn { get; set; }
    }
}

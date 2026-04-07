using ServerMysticArea.GameServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.RoomAll
{
    public class RoomManager
    {
        private Dictionary<int, Room> rooms= new Dictionary<int, Room>();
        private int nextRoomId = 1;
        public void UpdateAllRooms()
        {
            var roomList = rooms.Values.ToList();

            foreach (var room in roomList)
            {
                room.Update();
            }
        }

        public void CreateRoom(PlayerSession host)
        {
            
            if (host.CurrentRoom != null) return ;

            Room room = new Room(nextRoomId++, host);
            rooms.Add(room.RoomId, room);
            host.CurrentRoom = room;
            GameSender.SendCreateRoom(host, room.RoomId);
             Console.WriteLine($"Người chơi {host.PlayerData.Nickname} đã tạo phòng {room.RoomId}");

        }
        public void RemovePlayerFromRoom(PlayerSession player)
        {
            if (player.CurrentRoom != null)
            {
                Room room = player.CurrentRoom;
                if (room.HostPlayer != null && room.HostPlayer.Session == player)
                {
                    player.CurrentRoom = null;
                    RemoveRoom(room.RoomId);
                    Console.WriteLine($"Người chơi {player.PlayerData.Nickname} đã rời phòng {room.RoomId}");
                }
                else if (room.GuestPlayer != null && room.GuestPlayer.Session == player)
                {
                    player.CurrentRoom=null;
                }
                if (room.HostPlayer == null && room.GuestPlayer == null)
                {
                    rooms.Remove(room.RoomId);
                    Console.WriteLine($"Phòng {room.RoomId} đã bị xóa vì không còn người chơi nào");
                }
            }
        }
        public void RemoveRoom(int roomId)
        {
            Room room;
            if (rooms.TryGetValue(roomId, out room))
            {
                if (room.HostPlayer != null)
                    room.HostPlayer.Session.CurrentRoom = null;

                if (room.GuestPlayer != null)
                    room.GuestPlayer.Session.CurrentRoom = null;

                rooms.Remove(roomId);
            }
        }
        public Room GetRoomById(int roomId)
        {
            Room room;
            if (rooms.TryGetValue(roomId, out room))
            {
                return room;
            }
            return null;
        }

        public void HandleDisconnect(PlayerSession player)
        {
            if (player == null) return;

            if (player.CurrentRoom != null)
            {
                player.CurrentRoom.HandleDisconnect(player, this);
            }
        }
    }
}

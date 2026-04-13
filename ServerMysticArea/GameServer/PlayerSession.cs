using ServerMysticArea.DB;
using ServerMysticArea.Player;
using ServerMysticArea.RoomAll;
using ServerMysticArea.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ServerMysticArea.GameServer
{
    public class PlayerSession
    {
        public int SessionId { get; }
        public Room CurrentRoom { get; set; }
        public TcpClient Client { get; }
        public NetworkStream Stream { get; }
        public BinaryReader reader { get; }
        public BinaryWriter writer { get; }
        public PlayerData PlayerData { get; private set; }
        public int PlayerId { get; private set; }
        public AccountData accountData { get; private set; }
      

        private readonly SessionManager _manager;
        private bool _isRunning;
        public PlayerSession(int sessionId, TcpClient client, SessionManager manager)
        {
            SessionId = sessionId;
            Client = client;
            Stream = client.GetStream();
            reader = new BinaryReader(Stream);
            writer = new BinaryWriter(Stream);
            _manager = manager;
        }
        public async Task StartAsync()
        {
            _isRunning = true;

            try
            {
                while (_isRunning)
                {
                    var message = await ReadMessageAsync();
                    
                    _manager.Dispatch(this, message);
                }
            }
            catch
            {
                Disconnect();
            }
        }


        private async Task<Message> ReadMessageAsync()
        {
            try
            {
                byte[] header = await ReadExactAsync(4);

                short opcode = BitConverter.ToInt16(header, 0);
                ushort length = BitConverter.ToUInt16(header, 2);

                if (length > 4096)
                    throw new Exception("Packet too large");

                byte[] payload = length > 0
                    ? await ReadExactAsync(length)
                    : Array.Empty<byte>();

                return new Message(opcode, payload);
            }
            catch (IOException)
            {
                Console.WriteLine($"[DISCONNECT] Session {SessionId}");
                CardRepository cardRepository = new CardRepository();
                cardRepository.SaveAllPlayerCards(this.PlayerData.PlayerId, this.PlayerData.playerCard.AllCard);
                cardRepository.SyncDeckCards( this.PlayerData.playerDeckCard);
                _manager.RemoveSession(this);
                throw;
            }
        }
        private async Task<byte[]> ReadExactAsync(int size)
        {
            byte[] buffer = new byte[size];
            int offset = 0;

            while (offset < size)
            {
                int read = await Stream.ReadAsync(buffer, offset, size - offset);

                if (read == 0)
                    throw new IOException("Client disconnected");

                offset += read;
            }

            return buffer;
        }
        public void SetAccountData(AccountData id)
        {
            accountData = id;
        }
        public void SetPlayerData(PlayerData playerData)
        {
            this.PlayerData = playerData;
        }
        public void Authenticate(int playerId)
        {
            PlayerId = playerId;
        }
        public void Disconnect()
        {
            _isRunning = false;
            _manager.RemoveSession(this);
            MainServer._roomManager.HandleDisconnect(this);
            Client.Close();
        }
        public void Send(Message m)
        {
            if (m == null || Client == null || !Client.Connected)
                return;

            try
            {
                byte[] payload = m.ToArray();
                ushort length = (ushort)(payload?.Length ?? 0);             
                Console.WriteLine($"[SEND] Opcode={m.Command} Length={length}");
                writer.Write(m.Command);              
                writer.Write(length);               
                if (length > 0)
                {
                    writer.Write(payload);
                }

                writer.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SEND ERROR] " + ex.Message);
            }
        }
    }
}

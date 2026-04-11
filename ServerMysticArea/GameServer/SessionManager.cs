using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.GameServer
{
    public class SessionManager
    {
        private readonly ConcurrentDictionary<int, PlayerSession> _sessions = new();
        private int _sessionIdCounter = 0;
        public GameRouter _GameRouter;

        public PlayerSession CreateSession(TcpClient client)
        {
            int sessionId = Interlocked.Increment(ref _sessionIdCounter);

            var session = new PlayerSession(sessionId, client, this);
            _sessions.TryAdd(sessionId, session);

            return session;
        }

        public void RemoveSession(PlayerSession session)
        {
            _sessions.TryRemove(session.SessionId, out _);

            Console.WriteLine($"Player {session.PlayerId} disconnected");

        }

        public PlayerSession? GetSession(int sessionId)
        {
            _sessions.TryGetValue(sessionId, out var session);
            return session;
        }

        public void Dispatch(PlayerSession session, Message message)
        {
            // Chuyển sang Game Logic Layer
            _GameRouter.Handle(session, message);
        }

        public void Broadcast(Message message)
        {
            foreach (var session in _sessions.Values)
            {
                session.Send(message);
            }
        }
    }
}

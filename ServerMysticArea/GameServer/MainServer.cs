using ServerMysticArea.GameServer;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerMysticArea.Server
{
    public static class MainServer
    {
        private static TcpListener _listener;
        private static SessionManager _sessionManager;
        private static GameRouter _gameRouter;
        static async Task Main(string[] args)
        {
            Console.Title = "Card Game Server";

            int port = 7777;
            _listener = new TcpListener(IPAddress.Any, port);
            _sessionManager = new SessionManager();
            _gameRouter = new GameRouter();
            _sessionManager._GameRouter = _gameRouter;
            _listener.Start();

            Console.WriteLine($"Server started on port {port}");
            Console.WriteLine("Waiting for connections...");

            while (true)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();

                    Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");

                    var session = _sessionManager.CreateSession(client);

                    _ = session.StartAsync(); // fire and forget
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] {ex.Message}");
                }
            }
        }
    }
}
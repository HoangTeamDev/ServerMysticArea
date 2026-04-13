using ServerMysticArea.Batte;
using ServerMysticArea.GameServer;
using ServerMysticArea.RoomAll;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerMysticArea.Server
{
    public static class MainServer
    {
        private static TcpListener _listener;
        private static SessionManager _sessionManager=new SessionManager();
        private static GameRouter _gameRouter=new GameRouter();
        public static RoomManager _roomManager=new RoomManager();
        public static ActionProcessor _actionProcessor=new ActionProcessor();
        public static BattleManager _battleManager=new BattleManager();
        public static CardInstanceManager _cardInstanceManager=new CardInstanceManager();
        public static RuleValidator _ruleValidator=new RuleValidator();
        public static TurnManager _turnManager=new TurnManager();
        public static ZoneManager _zoneManager=new ZoneManager();
        public static GameRouterBattle gameRouterBattle=new GameRouterBattle();
        private static bool isRunning = true;
        static async Task Main(string[] args)
        {
            Console.Title = "Card Game Server";

            int port = 7777;
            _listener = new TcpListener(IPAddress.Any, port);
            _sessionManager = new SessionManager();
            _gameRouter = new GameRouter();
            _roomManager = new RoomManager();
            _battleManager = new BattleManager();
            _cardInstanceManager = new CardInstanceManager();
            _ruleValidator = new RuleValidator();
            _turnManager = new TurnManager();
            _zoneManager = new ZoneManager();
            _actionProcessor = new ActionProcessor();
            gameRouterBattle = new GameRouterBattle();
            _sessionManager._GameRouter = _gameRouter;
            _sessionManager._Battle = gameRouterBattle;
            _listener.Start();
            CardManager.LoadAll();
            Console.WriteLine($"Server started on port {port}");
            Console.WriteLine("Waiting for connections...");
            new Thread(() =>
            {
                while (isRunning)
                {
                    _roomManager.UpdateAllRooms();
                    Thread.Sleep(200);
                }
            }).Start();
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
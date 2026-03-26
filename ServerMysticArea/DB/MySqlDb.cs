using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.DB
{
    using MySqlConnector;
    public static class MySqlDb
    {
        static string connString =
            "Server=localhost;Database=game_server;Uid=root;Pwd=123456;";

        public static MySqlConnection GetConnection()
            => new MySqlConnection(connString);
    }
}

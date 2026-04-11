using MySqlConnector;
using ServerMysticArea.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.DB
{
    public static class AccountRepository
    {
        public static async Task<AccountData> GetByUsername(string username, string pass)
        {
            using var conn = MySqlDb.GetConnection();
            await conn.OpenAsync();

            var cmd = new MySqlCommand(
                "SELECT * FROM Users WHERE username=@u AND PasswordHash=@p", conn);

            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", pass);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new AccountData
            {
                AccountId = reader.GetInt32("ID"),
                Username = reader.GetString("Username"),
                PasswordHash = reader.GetString("PasswordHash")
            };
        }

        public static async Task InsertAccountAsync(int id, string name, string pass)
        {
            using var conn = MySqlDb.GetConnection();
            await conn.OpenAsync();
           
            string query = "INSERT INTO users (Id,Username, PasswordHash,Email ) VALUES (@Id,@name, @pass,@mail)";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@pass", pass);
            cmd.Parameters.AddWithValue("@mail", "tryx@123.CompareTo");

            int rows = await cmd.ExecuteNonQueryAsync();

            Console.WriteLine($"Inserted {rows} row(s)");
        }
    }
}

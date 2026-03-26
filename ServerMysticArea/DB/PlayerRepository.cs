using MySqlConnector;
using ServerMysticArea.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.DB
{
    public static class PlayerRepository
    {
        public static async Task<PlayerData> GetByAccountId(int accountId)
        {
            using var conn = MySqlDb.GetConnection();
            await conn.OpenAsync();

            var cmd = new MySqlCommand(
                "SELECT * FROM players WHERE AccountId=@id", conn);

            cmd.Parameters.AddWithValue("@Id", accountId);

            var reader = await cmd.ExecuteReaderAsync();
            if (!reader.Read()) return null;

            return new PlayerData
            {
                PlayerId = reader.GetInt32("Id"),
                AccountId = accountId,
                Nickname = reader.GetString("nickname"),

                Level = reader.GetInt32("level"),
                Exp = reader.GetInt64("exp"),
                Gold = reader.GetInt32("gold"),
                Diamond = reader.GetInt32("diamond")


            };
        }

        public static async Task InsertPlayerAsync(PlayerData player)
        {
            using var conn = MySqlDb.GetConnection();
            await conn.OpenAsync();

            string query = @"
        INSERT INTO players 
        (Id, AccountId, nickname, level, exp, gold,diamond)
        VALUES 
        (@id, @accountId, @name, @level, @exp, @gold,@diamond)";

            using var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@id", player.PlayerId);
            cmd.Parameters.AddWithValue("@accountId", player.AccountId);
            cmd.Parameters.AddWithValue("@name", player.Nickname);
            cmd.Parameters.AddWithValue("@level", player.Level);
            cmd.Parameters.AddWithValue("@exp", player.Exp);
            cmd.Parameters.AddWithValue("@gold", player.Gold);
            cmd.Parameters.AddWithValue("@diamond", player.Diamond);

            int rows = await cmd.ExecuteNonQueryAsync();

            Console.WriteLine($"Inserted {rows} player(s)");
        }
    }
}

using MySqlConnector;
using UnityServer.Models;
using UnityServer.Player;
using UnityServer.Repository.Interface;

namespace UnityServer.Repository
{
    public class UserRepository : IUserRepository
    {
        private const string SqlHasCharacter =
            "SELECT COUNT(*) FROM user WHERE account_id = @accountId";
        private const string SqlCreateCharacter =
            "INSERT INTO user (account_id, level, exp, hp, mp) VALUES (@accountId, 1, 0, 100, 50)";
        private const string SqlGetCharacter =
            "SELECT * FROM user WHERE account_id = @accountId";
        private const string SqlSaveCharacterStats =
            "UPDATE user SET level = @level, exp = @exp, hp = @hp, mp = @mp, map = @map WHERE account_id = @accountId";

        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public bool HasCharacter(int accountId)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(SqlHasCharacter, connection);
            command.Parameters.AddWithValue("@accountId", accountId);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        public void CreateCharacter(int accountId)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(SqlCreateCharacter, connection);
            command.Parameters.AddWithValue("@accountId", accountId);
            command.ExecuteNonQuery();
        }

        public CharacterData? GetCharacterByAccountId(int accountId)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(SqlGetCharacter, connection);
            command.Parameters.AddWithValue("@accountId", accountId);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
                return null;

            return new CharacterData(
                reader.GetInt32("level"),
                reader.GetInt32("exp"),
                reader.GetInt32("hp"),
                reader.GetInt32("mp"),
                reader.GetInt32("map")
            );
        }

        public void SaveCharacterStats(int accountId, PlayerStats stats, int map)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(SqlSaveCharacterStats, connection);
            command.Parameters.AddWithValue("@accountId", accountId);
            command.Parameters.AddWithValue("@level", stats.Level);
            command.Parameters.AddWithValue("@exp", stats.Exp);
            command.Parameters.AddWithValue("@hp", stats.Hp);
            command.Parameters.AddWithValue("@mp", stats.Mp);
            command.Parameters.AddWithValue("@map", map);
            command.ExecuteNonQuery();
        }
    }
}

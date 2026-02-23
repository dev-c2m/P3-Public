using MySqlConnector;
using UnityServer.Models;
using UnityServer.Repository.Interface;

namespace UnityServer.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private const string SqlIsLoginIdExists =
            "SELECT COUNT(*) FROM account WHERE user_id = @userId";
        private const string SqlCreateAccount =
            "INSERT INTO account (user_id, user_pw, nickname) VALUES (@userId, @userPw, @nickname); SELECT LAST_INSERT_ID()";
        private const string SqlValidateLogin =
            "SELECT id, nickname FROM account WHERE user_id = @userId AND user_pw = @userPw";
        private const string SqlIsNicknameExists =
            "SELECT COUNT(*) FROM account WHERE nickname = @nickname";

        private readonly string _connectionString;

        public AccountRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool IsLoginIdExists(string loginId)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            using MySqlCommand command = new MySqlCommand(SqlIsLoginIdExists, connection);
            command.Parameters.AddWithValue("@userId", loginId);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        public int CreateAccount(string loginId, string password, string nickname)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            using MySqlCommand command = new MySqlCommand(SqlCreateAccount, connection);
            command.Parameters.AddWithValue("@userId", loginId);
            command.Parameters.AddWithValue("@userPw", password);
            command.Parameters.AddWithValue("@nickname", nickname);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public LoginResult? ValidateLogin(string loginId, string password)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            using MySqlCommand command = new MySqlCommand(SqlValidateLogin, connection);
            command.Parameters.AddWithValue("@userId", loginId);
            command.Parameters.AddWithValue("@userPw", password);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
                return null;

            return new LoginResult(
                reader.GetInt32("id"),
                reader.GetString("nickname")
            );
        }

        public bool IsNicknameExists(string nickname)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            using MySqlCommand command = new MySqlCommand(SqlIsNicknameExists, connection);
            command.Parameters.AddWithValue("@nickname", nickname);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
    }
}

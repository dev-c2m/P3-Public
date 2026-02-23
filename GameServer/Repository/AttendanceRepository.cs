using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityServer.Models;
using UnityServer.Repository.Interface;

namespace UnityServer.Repository
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private const string SqlCheckAttendance =
            "INSERT INTO user_attendance_record (account_id, attendance_id, day) VALUES (@accountId, @attendanceId, @day)";
        private const string SqlGetLastRecord =
            "SELECT * FROM user_attendance_record WHERE account_id = @accountId AND attendance_id = @attendanceId ORDER BY day DESC LIMIT 1";
        private const string SqlExistsByDay =
            "SELECT 1 FROM user_attendance_record WHERE account_id = @accountId AND attendance_id = @attendanceId AND day = @day";

        private readonly string _connectionString;

        public AttendanceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CheckAttendance(int accountId, int attendanceId, int day)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            using MySqlCommand command = new MySqlCommand(SqlCheckAttendance, connection);
            command.Parameters.AddWithValue("@accountId", accountId);
            command.Parameters.AddWithValue("@attendanceId", attendanceId);
            command.Parameters.AddWithValue("@day", day);

            command.ExecuteNonQuery();
        }

        public int GetLastAttendanceDayCount(int accountId, int attendanceId)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            using MySqlCommand command = new MySqlCommand(SqlGetLastRecord, connection);
            command.Parameters.AddWithValue("@accountId", accountId);
            command.Parameters.AddWithValue("@attendanceId", attendanceId);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
                return 0;

            return reader.GetInt32("day");
        }

        public bool CanCheckInToday(int accountId, int attendanceId)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            int totalDays = DataManager.GetAttendanceTotalDays(attendanceId);
            using MySqlCommand command = new MySqlCommand(SqlGetLastRecord, connection);
            command.Parameters.AddWithValue("@accountId", accountId);
            command.Parameters.AddWithValue("@attendanceId", attendanceId);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
                return true;

            return reader.GetDateTime("created_at").Date != DateTime.Now.Date;
        }

        public void TryCheckAttendance(int accountId, int attendanceId, int day)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            using MySqlCommand selectCommand = new MySqlCommand(SqlExistsByDay, connection);
            selectCommand.Parameters.AddWithValue("@accountId", accountId);
            selectCommand.Parameters.AddWithValue("@attendanceId", attendanceId);
            selectCommand.Parameters.AddWithValue("@day", day);

            if (selectCommand.ExecuteScalar() != null)
                return;

            using MySqlCommand insertCommand = new MySqlCommand(SqlCheckAttendance, connection);
            insertCommand.Parameters.AddWithValue("@accountId", accountId);
            insertCommand.Parameters.AddWithValue("@attendanceId", attendanceId);
            insertCommand.Parameters.AddWithValue("@day", day);
            insertCommand.ExecuteNonQuery();
        }

        public AttendanceResult GetAttendanceDayCountAndCanCheckIn(int accountId, int attendanceId)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            using MySqlCommand command = new MySqlCommand(SqlGetLastRecord, connection);
            command.Parameters.AddWithValue("@accountId", accountId);
            command.Parameters.AddWithValue("@attendanceId", attendanceId);
           
            using var reader = command.ExecuteReader();          
            if (!reader.Read())
                return new AttendanceResult(0, true);

            return new AttendanceResult(
                reader.GetInt32("day"),
                reader.GetDateTime("created_at").Date != DateTime.Now.Date
            );
        }
    }
}

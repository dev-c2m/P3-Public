using MySqlConnector;
using UnityServer.Models;
using UnityServer.Repository.Interface;

namespace UnityServer.Repository
{
    public class InventoryRepository : IInventoryRepository
    {
        private const string SqlGetItems =
            "SELECT slot_index, item_id, qty FROM inventory WHERE account_id = @accountId";
        private const string SqlInsertItem =
            "INSERT INTO inventory (account_id, slot_index, item_id, qty) VALUES (@accountId, @slotIndex, @itemId, @qty) " +
            "ON DUPLICATE KEY UPDATE item_id = @itemId, qty = @qty";
        private const string SqlUpdateItem =
            "UPDATE inventory SET qty = @qty WHERE account_id = @accountId AND slot_index = @slotIndex";
        private const string SqlDeleteItem =
            "DELETE FROM inventory WHERE account_id = @accountId AND slot_index = @slotIndex";
        private const string SqlMoveItem =
            "UPDATE inventory SET slot_index = @toSlot WHERE account_id = @accountId AND slot_index = @fromSlot";
        private const string SqlSwapToTemp =
            "UPDATE inventory SET slot_index = -1 WHERE account_id = @accountId AND slot_index = @slotA";
        private const string SqlSwapBtoA =
            "UPDATE inventory SET slot_index = @slotA WHERE account_id = @accountId AND slot_index = @slotB";
        private const string SqlSwapTempToB =
            "UPDATE inventory SET slot_index = @slotB WHERE account_id = @accountId AND slot_index = -1";

        private readonly string _connectionString;

        public InventoryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<InventoryItem> GetItems(int accountId)
        {
            var items = new List<InventoryItem>();

            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(SqlGetItems, connection);
            command.Parameters.AddWithValue("@accountId", accountId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new InventoryItem(
                    reader.GetInt32("item_id"),
                    reader.GetInt32("slot_index"),
                    reader.GetInt32("qty")
                ));
            }

            return items;
        }

        public void InsertItem(int accountId, int slotIndex, int itemId, int qty)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(SqlInsertItem, connection);
            command.Parameters.AddWithValue("@accountId", accountId);
            command.Parameters.AddWithValue("@slotIndex", slotIndex);
            command.Parameters.AddWithValue("@itemId", itemId);
            command.Parameters.AddWithValue("@qty", qty);
            command.ExecuteNonQuery();
        }

        public void UpdateItem(int accountId, int slotIndex, int qty)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(SqlUpdateItem, connection);
            command.Parameters.AddWithValue("@accountId", accountId);
            command.Parameters.AddWithValue("@slotIndex", slotIndex);
            command.Parameters.AddWithValue("@qty", qty);
            command.ExecuteNonQuery();
        }

        public void DeleteItem(int accountId, int slotIndex)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(SqlDeleteItem, connection);
            command.Parameters.AddWithValue("@accountId", accountId);
            command.Parameters.AddWithValue("@slotIndex", slotIndex);
            command.ExecuteNonQuery();
        }

        public void MoveItem(int accountId, int fromSlot, int toSlot)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(SqlMoveItem, connection);
            command.Parameters.AddWithValue("@accountId", accountId);
            command.Parameters.AddWithValue("@fromSlot", fromSlot);
            command.Parameters.AddWithValue("@toSlot", toSlot);
            command.ExecuteNonQuery();
        }

        public void SwapItems(int accountId, int slotA, int slotB)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                using var cmd1 = new MySqlCommand(SqlSwapToTemp, connection, transaction);
                cmd1.Parameters.AddWithValue("@accountId", accountId);
                cmd1.Parameters.AddWithValue("@slotA", slotA);
                cmd1.ExecuteNonQuery();

                using var cmd2 = new MySqlCommand(SqlSwapBtoA, connection, transaction);
                cmd2.Parameters.AddWithValue("@accountId", accountId);
                cmd2.Parameters.AddWithValue("@slotA", slotA);
                cmd2.Parameters.AddWithValue("@slotB", slotB);
                cmd2.ExecuteNonQuery();

                using var cmd3 = new MySqlCommand(SqlSwapTempToB, connection, transaction);
                cmd3.Parameters.AddWithValue("@accountId", accountId);
                cmd3.Parameters.AddWithValue("@slotB", slotB);
                cmd3.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

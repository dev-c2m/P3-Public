using StackExchange.Redis;
using UnityServer.Models;
using UnityServer.Player;

namespace UnityServer
{
    public class RedisManager
    {
        private const string CharacterKey = "char:{0}";
        private const string InventoryKey = "inv:{0}";
        private const string AttendanceKey = "att:{0}";

        private readonly ConnectionMultiplexer connection;
        private readonly IDatabase db;

        public RedisManager(string connectionString)
        {
            connection = ConnectionMultiplexer.Connect(connectionString);
            db = connection.GetDatabase();
            Logger.Success("[RedisManager] Redis 연결 완료");
        }

        public void SaveCharacter(int accountId, PlayerStats stats, int mapId)
        {
            string key = string.Format(CharacterKey, accountId);
            HashEntry[] entries = new HashEntry[]
            {
                new HashEntry("level", stats.Level),
                new HashEntry("exp", stats.Exp),
                new HashEntry("hp", stats.Hp),
                new HashEntry("mp", stats.Mp),
                new HashEntry("map", mapId)
            };
            db.HashSet(key, entries);
        }

        public void SaveCharacterFireAndForget(int accountId, PlayerStats stats, int mapId)
        {
            string key = string.Format(CharacterKey, accountId);
            HashEntry[] entries = new HashEntry[]
            {
                new HashEntry("level", stats.Level),
                new HashEntry("exp", stats.Exp),
                new HashEntry("hp", stats.Hp),
                new HashEntry("mp", stats.Mp),
                new HashEntry("map", mapId)
            };
            db.HashSet(key, entries, CommandFlags.FireAndForget);
        }

        public CharacterData? LoadCharacter(int accountId)
        {
            string key = string.Format(CharacterKey, accountId);
            HashEntry[] entries = db.HashGetAll(key);

            if (entries.Length == 0)
                return null;

            int level = 0;
            int exp = 0;
            int hp = 0;
            int mp = 0;
            int map = 1;

            foreach (HashEntry entry in entries)
            {
                switch (entry.Name.ToString())
                {
                    case "level": level = (int)entry.Value; break;
                    case "exp": exp = (int)entry.Value; break;
                    case "hp": hp = (int)entry.Value; break;
                    case "mp": mp = (int)entry.Value; break;
                    case "map": map = (int)entry.Value; break;
                }
            }

            return new CharacterData(level, exp, hp, mp, map);
        }

        public void SaveInventory(int accountId, Dictionary<int, InventoryItem> inventory)
        {
            string key = string.Format(InventoryKey, accountId);

            db.KeyDelete(key);

            if (inventory.Count == 0)
                return;

            HashEntry[] entries = new HashEntry[inventory.Count];
            int index = 0;
            foreach (InventoryItem item in inventory.Values)
            {
                entries[index] = new HashEntry(item.SlotIndex.ToString(), $"{item.ItemId}:{item.Quantity}");
                index++;
            }
            db.HashSet(key, entries);
        }

        public void SaveInventoryFireAndForget(int accountId, Dictionary<int, InventoryItem> inventory)
        {
            string key = string.Format(InventoryKey, accountId);

            db.KeyDelete(key, CommandFlags.FireAndForget);

            if (inventory.Count == 0)
                return;

            HashEntry[] entries = new HashEntry[inventory.Count];
            int index = 0;
            foreach (InventoryItem item in inventory.Values)
            {
                entries[index] = new HashEntry(item.SlotIndex.ToString(), $"{item.ItemId}:{item.Quantity}");
                index++;
            }
            db.HashSet(key, entries, CommandFlags.FireAndForget);
        }

        public List<InventoryItem> LoadInventory(int accountId)
        {
            string key = string.Format(InventoryKey, accountId);
            HashEntry[] entries = db.HashGetAll(key);
            List<InventoryItem> items = new List<InventoryItem>();

            foreach (HashEntry entry in entries)
            {
                int slotIndex = int.Parse(entry.Name.ToString());
                string[] parts = entry.Value.ToString().Split(':');
                int itemId = int.Parse(parts[0]);
                int quantity = int.Parse(parts[1]);
                items.Add(new InventoryItem(itemId, slotIndex, quantity));
            }

            return items;
        }

        public void UpdateInventorySlot(int accountId, int slotIndex, int itemId, int quantity)
        {
            string key = string.Format(InventoryKey, accountId);
            db.HashSet(key, slotIndex.ToString(), $"{itemId}:{quantity}", flags: CommandFlags.FireAndForget);
        }

        public void DeleteInventorySlot(int accountId, int slotIndex)
        {
            string key = string.Format(InventoryKey, accountId);
            db.HashDelete(key, slotIndex.ToString(), CommandFlags.FireAndForget);
        }

        public void SwapInventorySlots(int accountId, int slotA, int slotB)
        {
            string key = string.Format(InventoryKey, accountId);
            RedisValue valueA = db.HashGet(key, slotA.ToString());
            RedisValue valueB = db.HashGet(key, slotB.ToString());

            if (valueA.HasValue && valueB.HasValue)
            {
                db.HashSet(key, slotA.ToString(), valueB, flags: CommandFlags.FireAndForget);
                db.HashSet(key, slotB.ToString(), valueA, flags: CommandFlags.FireAndForget);
            }
            else if (valueA.HasValue)
            {
                db.HashSet(key, slotB.ToString(), valueA, flags: CommandFlags.FireAndForget);
                db.HashDelete(key, slotA.ToString(), CommandFlags.FireAndForget);
            }
            else if (valueB.HasValue)
            {
                db.HashSet(key, slotA.ToString(), valueB, flags: CommandFlags.FireAndForget);
                db.HashDelete(key, slotB.ToString(), CommandFlags.FireAndForget);
            }
        }

        public void MoveInventorySlot(int accountId, int fromSlot, int toSlot)
        {
            string key = string.Format(InventoryKey, accountId);
            RedisValue value = db.HashGet(key, fromSlot.ToString());
            if (value.HasValue)
            {
                db.HashSet(key, toSlot.ToString(), value, flags: CommandFlags.FireAndForget);
                db.HashDelete(key, fromSlot.ToString(), CommandFlags.FireAndForget);
            }
        }

        public void SaveAttendanceFireAndForget(int accountId, int attendanceId, int dayCount, bool isCheckedToday)
        {
            string key = string.Format(AttendanceKey, accountId);
            db.HashSet(key, attendanceId.ToString(), $"{dayCount}:{(isCheckedToday ? 1 : 0)}", flags: CommandFlags.FireAndForget);
        }

        public void SaveAttendance(int accountId, int attendanceId, int dayCount, bool isCheckedToday)
        {
            string key = string.Format(AttendanceKey, accountId);
            db.HashSet(key, attendanceId.ToString(), $"{dayCount}:{(isCheckedToday ? 1 : 0)}");
        }

        public List<PlayerAttendance> LoadAttendance(int accountId)
        {
            string key = string.Format(AttendanceKey, accountId);
            HashEntry[] entries = db.HashGetAll(key);
            List<PlayerAttendance> attendances = new List<PlayerAttendance>();

            foreach (HashEntry entry in entries)
            {
                int attendanceId = int.Parse(entry.Name.ToString());
                string[] parts = entry.Value.ToString().Split(':');
                int dayCount = int.Parse(parts[0]);
                bool isCheckedToday = parts[1] == "1";
                attendances.Add(new PlayerAttendance(attendanceId, dayCount, isCheckedToday));
            }

            return attendances;
        }

        public void DeletePlayerData(int accountId)
        {
            db.KeyDelete(string.Format(CharacterKey, accountId), CommandFlags.FireAndForget);
            db.KeyDelete(string.Format(InventoryKey, accountId), CommandFlags.FireAndForget);
            db.KeyDelete(string.Format(AttendanceKey, accountId), CommandFlags.FireAndForget);
        }

        public void Close()
        {
            connection.Close();
            Logger.Info("[RedisManager] Redis 연결 종료");
        }
    }
}

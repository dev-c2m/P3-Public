using System.Text.Json;
using UnityServer.Data;
using UnityServer.Models;

namespace UnityServer
{
    public static class DataManager
    {
        private static Dictionary<int, LevelData> levels = new Dictionary<int, LevelData>();
        private static Dictionary<int, ItemData> items = new Dictionary<int, ItemData>();
        private static Dictionary<int, AttendanceData> attendances = new Dictionary<int, AttendanceData>();
        private static Dictionary<int, SkillData> skills = new Dictionary<int, SkillData>();
        private static Dictionary<int, MapData> maps = new Dictionary<int, MapData>();
        private static Dictionary<int, MonsterData> monsters = new Dictionary<int, MonsterData>();

        public static void LoadAll()
        {
            string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Share", "Data");

            LoadLevels(Path.Combine(dataPath, "LevelData.json"));
            LoadItems(Path.Combine(dataPath, "ItemData.json"));
            LoadAttendance(Path.Combine(dataPath, "AttendanceData.json"));
            LoadSkills(Path.Combine(dataPath, "SkillData.json"));
            LoadMonsters(Path.Combine(dataPath, "MonsterData.json"));
            LoadMaps(Path.Combine(dataPath, "MapData.json"));

            Logger.Success("[DataManager] 모든 데이터 로드 완료");
        }

        private static void LoadJson<T>(string filePath, string arrayPropertyName, Dictionary<int, T> dictionary) where T : IDataWithId
        {
            try
            {
                string json = File.ReadAllText(filePath);

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                JsonElement root = JsonSerializer.Deserialize<JsonElement>(json, options);
                JsonElement dataArray = root.GetProperty(arrayPropertyName);
                List<T>? list = dataArray.Deserialize<List<T>>(options);

                if (list == null)
                    throw new Exception($"{arrayPropertyName} 파싱 실패");

                foreach (T info in list)
                {
                    if (dictionary.ContainsKey(info.Id))
                        throw new Exception($"{arrayPropertyName} 데이터 중복 Id 발견: {info.Id}");

                    dictionary[info.Id] = info;
                }
                Logger.Success($"[DataManager] {arrayPropertyName} 데이터 로드 완료: {dictionary.Count}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        private static void LoadLevels(string filePath)
        {
            try
            {
                LoadJson<LevelData>(filePath, "levels", levels);

                foreach (LevelData info in levels.Values)
                {
                    if (info.Level > Constants.MaxLevel)
                        throw new Exception($"레벨 데이터 불일치, 데이터 확인 필요-> Data: {info.Level}, MaxLevel {Constants.MaxLevel}");
                }

                for (int i = 1; i <= Constants.MaxLevel; i++)
                {
                    if (!levels.ContainsKey(i))
                        throw new Exception($"레벨 {i} 데이터 누락");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        private static void LoadItems(string filePath)
        {
            try
            {
                LoadJson<ItemData>(filePath, "items", items);

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        private static void LoadSkills(string filePath)
        {
            try
            {
                LoadJson<SkillData>(filePath, "skills", skills);

                foreach (SkillData skill in skills.Values)
                {
                    if (skill.ProjectileSpeed <= 0)
                        throw new Exception($"스킬 데이터 투사체 속도 오류, 데이터 확인 필요-> Id: {skill.Id}, ProjectileSpeed: {skill.ProjectileSpeed}");

                    if (skill.Range <= 0)
                        throw new Exception($"스킬 데이터 사거리 오류, 데이터 확인 필요-> Id: {skill.Id}, Range: {skill.Range}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        private static void LoadMonsters(string filePath)
        {
            try
            {
                LoadJson<MonsterData>(filePath, "monsters", monsters);

                foreach (MonsterData monster in monsters.Values)
                {
                    foreach (MonsterDropData drop in monster.Drops)
                    {
                        if (!items.ContainsKey(drop.ItemId))
                            throw new Exception($"몬스터 드랍 아이템 Id 불일치, 데이터 확인 필요-> MonsterId: {monster.Id}, ItemId: {drop.ItemId}");

                        if (drop.DropRate < 0 || drop.DropRate > 100)
                            throw new Exception($"몬스터 드랍 확률 범위 오류, 데이터 확인 필요-> MonsterId: {monster.Id}, ItemId: {drop.ItemId}, DropRate: {drop.DropRate}");

                        if (drop.Quantity <= 0)
                            throw new Exception($"몬스터 드랍 수량 오류, 데이터 확인 필요-> MonsterId: {monster.Id}, ItemId: {drop.ItemId}, Quantity: {drop.Quantity}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        private static void LoadMaps(string filePath)
        {
            try
            {
                LoadJson<MapData>(filePath, "maps", maps);

                if (!maps.ContainsKey(Constants.DefaultMapId))
                    throw new Exception($"기본 맵 데이터 누락, 데이터 확인 필요-> DefaultMapId: {Constants.DefaultMapId}");

                foreach (MapData map in maps.Values)
                {
                    foreach (int nextMapId in map.NextMap)
                    {
                        if (!maps.ContainsKey(nextMapId))
                            throw new Exception($"맵 데이터 NextMap 참조 오류, 데이터 확인 필요-> MapId: {map.Id}, NextMapId: {nextMapId}");
                    }

                    foreach (MapMonsterSpawn spawn in map.MonsterSpawns)
                    {
                        if (!monsters.ContainsKey(spawn.MonsterId))
                            throw new Exception($"맵 데이터 MonsterSpawns 참조 오류, 데이터 확인 필요-> MapId: {map.Id}, MonsterId: {spawn.MonsterId}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        private static void LoadAttendance(string filePath)
        {
            try
            {
                LoadJson<AttendanceData>(filePath, "attendances", attendances);

                foreach (AttendanceData info in attendances.Values)
                {                    
                    foreach (AttendanceRewardData reward in info.Rewards)
                    {
                        if (!items.ContainsKey(reward.ItemId))
                            throw new Exception($"출석 데이터 보상 아이템 Id 불일치, 데이터 확인 필요-> AttendanceId: {info.Id}, ItemId: {reward.ItemId}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        #region 레벨 헬퍼

        public static int GetRequiredExp(int level)
        {
            return levels[level].RequiredExp;
        }

        public static int GetNextLevelRequiredExp(int level)
        {
            if (level >= Constants.MaxLevel)
                return GetRequiredExp(Constants.MaxLevel);

            return levels[level + 1].RequiredExp;
        }

        public static int GetMaxHp(int level)
        {
            return levels[level].MaxHp;
        }

        public static int GetMaxMp(int level)
        {
            return levels[level].MaxMp;
        }

        public static int CalculateLevel(int totalExp)
        {
            int level = 1;
            for (int i = Constants.MaxLevel; i >= 1; i--)
            {
                if (totalExp >= GetRequiredExp(i))
                {
                    level = i;
                    break;
                }
            }
            return level;
        }

        #endregion

        #region 스킬 헬퍼

        public static SkillData? GetSkillData(int skillId)
        {
            return skills.TryGetValue(skillId, out SkillData data) ? data : null;
        }

        #endregion

        #region 맵 헬퍼

        public static MapData? GetMapData(int mapId)
        {
            return maps.TryGetValue(mapId, out MapData data) ? data : null;
        }

        public static bool CanMoveToMap(int currentMapId, int targetMapId)
        {
            if (!maps.TryGetValue(currentMapId, out MapData currentMap))
                return false;

            return currentMap.NextMap.Contains(targetMapId);
        }

        public static List<MapData> GetAllMaps()
        {
            return maps.Values.ToList();
        }

        #endregion

        #region 몬스터 헬퍼

        public static MonsterData? GetMonsterData(int monsterId)
        {
            return monsters.TryGetValue(monsterId, out MonsterData data) ? data : null;
        }

        #endregion

        #region 아이템 헬퍼

        public static ItemData? GetItemData(int itemId)
        {
            return items.TryGetValue(itemId, out ItemData data) ? data : null;
        }

        #endregion

        #region 출석 헬퍼

        public static int GetAttendanceTotalDays(int attendanceId)
        {
            return attendances[attendanceId].EndDate.Subtract(attendances[attendanceId].StartDate).Days + 1;
        }

        public static List<int> GetAllAvailableAttendance()
        {
            List<int> result = new List<int>();

            foreach (AttendanceData attendance in attendances.Values)
            {
                if (attendance.StartDate <= DateTime.Now && attendance.EndDate >= DateTime.Now)
                {
                    result.Add(attendance.Id);
                }
            }

            return result;
        }

        public static bool IsAttendanceAvailable(int attendanceId)
        {
            if (!attendances.ContainsKey(attendanceId))
                return false;

            AttendanceData attendance = attendances[attendanceId];
            return attendance.StartDate <= DateTime.Now && attendance.EndDate >= DateTime.Now;
        }

        public static AttendanceRewardResult? GetAttendanceReward(int attendanceId, int day)
        {
            if (attendances.TryGetValue(attendanceId, out AttendanceData attendance))
            {
                AttendanceRewardData rewardData = attendance.Rewards[day];
                return new AttendanceRewardResult(rewardData.ItemId, rewardData.Quantity);
            }

            return null;
        }

        #endregion
    }
}

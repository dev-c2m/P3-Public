using System.Numerics;
using UnityServer.Data;
using UnityServer.Models;
using UnityServer.Share;

namespace UnityServer
{
    class MonsterManager
    {
        private Dictionary<int, MonsterInstance> monsters = new Dictionary<int, MonsterInstance>();
        private Dictionary<int, List<MonsterInstance>> mapMonsters = new Dictionary<int, List<MonsterInstance>>();
        private Dictionary<int, MapMonsterSpawn> monsterSpawnInfo = new Dictionary<int, MapMonsterSpawn>();
        private int nextUniqueId = 1;
        private readonly Random random = new Random();

        public void Initialize()
        {
            List<MapData> allMaps = DataManager.GetAllMaps();

            foreach (MapData map in allMaps)
            {
                if (map.MonsterSpawns == null || map.MonsterSpawns.Count == 0)
                    continue;

                mapMonsters[map.Id] = new List<MonsterInstance>();

                foreach (MapMonsterSpawn spawn in map.MonsterSpawns)
                {
                    MonsterData? monsterData = DataManager.GetMonsterData(spawn.MonsterId);
                    if (monsterData == null)
                        continue;

                    List<MonsterInstance> spawned = SpawnMonsters(map, spawn, monsterData, spawn.Count);
                    mapMonsters[map.Id].AddRange(spawned);
                }

                Logger.Success($"[MonsterManager] 맵 {map.Id}({map.Name}) 몬스터 {mapMonsters[map.Id].Count}마리 스폰 완료");
            }

            Logger.Success($"[MonsterManager] 전체 몬스터 {monsters.Count}마리 초기화 완료");
        }

        public List<MonsterInstance> GetMonstersInMap(int mapId)
        {
            if (!mapMonsters.TryGetValue(mapId, out List<MonsterInstance>? list))
                return new List<MonsterInstance>();

            return list.Where(m => !m.IsDead).ToList();
        }

        public void RemoveMonster(int uniqueId)
        {
            if (monsters.TryGetValue(uniqueId, out MonsterInstance? monster))
            {
                monsters.Remove(uniqueId);
                monsterSpawnInfo.Remove(uniqueId);
                if (mapMonsters.TryGetValue(monster.MapId, out List<MonsterInstance>? list))
                {
                    list.Remove(monster);
                }
            }
        }

        public void ProcessMonsterMovement()
        {
            long nowMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            DateTime now = DateTime.UtcNow;

            foreach (MonsterInstance monster in monsters.Values)
            {
                if (monster.IsDead)
                    continue;

                if (monster.ArriveTimeMs > 0 && nowMs >= monster.ArriveTimeMs)
                {
                    monster.Arrived();
                }

                if (now < monster.NextMoveAt)
                    continue;

                if (!monsterSpawnInfo.TryGetValue(monster.UniqueId, out MapMonsterSpawn? spawn))
                    continue;

                float targetX = spawn.SpawnX + (float)(random.NextDouble() * 2 - 1) * spawn.SpawnRange;
                float targetZ = spawn.SpawnZ + (float)(random.NextDouble() * 2 - 1) * spawn.SpawnRange;
                Vector3 nextPos = new Vector3(targetX, monster.Pos.Y, targetZ);
                float distance = monster.Distance(nextPos);
                long startTime = nowMs;
                long arriveTime = startTime + (long)(distance / monster.MoveSpeed * 1000);
                DateTime arriveDateTime = DateTimeOffset.FromUnixTimeMilliseconds(arriveTime).UtcDateTime;
                DateTime nextMoveAt = arriveDateTime.AddSeconds(random.Next(Constants.MonsterMoveMinWait, Constants.MonsterMoveMaxWait + 1));

                monster.UpdateNextMove(nextPos, startTime, arriveTime, nextMoveAt);

                IEnumerable<Session> sessions = Program.SessionManager.GetLoggedInSessionsInMap(monster.MapId);
                byte[] notify = PacketWriter.MonsterMoveNotify(monster.UniqueId, monster.Pos, nextPos, startTime, arriveTime);
                foreach (Session session in sessions)
                {
                    session.SendAsync(notify);
                }
            }
        }

        public void RespawnMonsters()
        {
            List<MapData> allMaps = DataManager.GetAllMaps();
            foreach (MapData map in allMaps)
            {
                if (map.MonsterSpawns == null || map.MonsterSpawns.Count == 0)
                    continue;

                foreach (MapMonsterSpawn spawn in map.MonsterSpawns)
                {
                    MonsterData? monsterData = DataManager.GetMonsterData(spawn.MonsterId);
                    if (monsterData == null)
                        continue;

                    int aliveCount = 0;
                    if (mapMonsters.TryGetValue(map.Id, out List<MonsterInstance>? list))
                    {
                        aliveCount = list.Count(m => m.MonsterId == spawn.MonsterId && !m.IsDead);
                    }
                    else
                    {
                        mapMonsters[map.Id] = new List<MonsterInstance>();
                        list = mapMonsters[map.Id];
                    }

                    int deficit = spawn.Count - aliveCount;
                    if (deficit <= 0)
                        continue;

                    List<MonsterInstance> spawnedInstances = SpawnMonsters(map, spawn, monsterData, deficit);
                    list.AddRange(spawnedInstances);

                    IEnumerable<Session> sessions = Program.SessionManager.GetLoggedInSessionsInMap(map.Id);
                    foreach (MonsterInstance instance in spawnedInstances)
                    {
                        byte[] notify = PacketWriter.MonsterSpawnNotify(map.Id, spawn.MonsterId, instance.UniqueId, instance.Pos);
                        foreach (Session session in sessions)
                        {
                            session.SendAsync(notify);
                        }
                    }

                    Logger.Info($"[MonsterManager] 맵 {map.Id} 몬스터(id:{spawn.MonsterId}) {deficit}마리 리스폰");
                }
            }
        }

        private List<MonsterInstance> SpawnMonsters(MapData map, MapMonsterSpawn spawn, MonsterData monsterData, int count)
        {
            List<MonsterInstance> spawned = new List<MonsterInstance>();
            for (int i = 0; i < count; i++)
            {
                float x = spawn.SpawnX + (float)(random.NextDouble() * 2 - 1) * spawn.SpawnRange;
                float z = spawn.SpawnZ + (float)(random.NextDouble() * 2 - 1) * spawn.SpawnRange;

                MonsterInstance instance = new MonsterInstance(nextUniqueId++, spawn.MonsterId, map.Id, monsterData.Hp, new Vector3(x, map.SpawnY, z));
                monsters[instance.UniqueId] = instance;
                monsterSpawnInfo[instance.UniqueId] = spawn;
                spawned.Add(instance);
            }
            return spawned;
        }

        public List<byte[]> GetSpawnNotifiesForMap(int mapId)
        {
            List<byte[]> result = new List<byte[]>();

            if (!mapMonsters.TryGetValue(mapId, out List<MonsterInstance>? list))
                return result;

            foreach (MonsterInstance monster in list)
            {
                if (monster.IsDead)
                    continue;

                result.Add(PacketWriter.MonsterSpawnNotify(mapId, monster.MonsterId, monster.UniqueId, monster.Pos));

                if (monster.ArriveTimeMs > 0)
                {
                    result.Add(PacketWriter.MonsterMoveNotify(monster.UniqueId, monster.Pos, monster.TargetPos, monster.StartTimeMs, monster.ArriveTimeMs));
                }
            }

            return result;
        }
    }
}

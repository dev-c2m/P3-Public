namespace UnityServer.Data
{
    public class MapData : IDataWithId
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<int> NextMap { get; set; } = new();
        public float SpawnX { get; set; }
        public float SpawnY { get; set; }
        public float SpawnZ { get; set; }
        public float MapSize { get; set; }
        public List<MapMonsterSpawn> MonsterSpawns { get; set; } = new();
    }
}

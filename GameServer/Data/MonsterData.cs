namespace UnityServer.Data
{
    public class MonsterData : IDataWithId
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Hp { get; set; }
        public int Damage { get; set; }
        public int Exp { get; set; }
        public List<MonsterDropData> Drops { get; set; } = new();
    }
}

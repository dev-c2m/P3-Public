namespace UnityServer.Data
{
    public class LevelData : IDataWithId
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public int RequiredExp { get; set; }
        public int MaxHp { get; set; }
        public int MaxMp { get; set; }
    }
}

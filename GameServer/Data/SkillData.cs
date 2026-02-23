namespace UnityServer.Data
{
    public class SkillData : IDataWithId
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int AttackType { get; set; }
        public int Damage { get; set; }
        public int MpCost { get; set; }
        public float Range { get; set; }
        public float ProjectileSpeed { get; set; }
        public float Cooldown { get; set; }
    }
}

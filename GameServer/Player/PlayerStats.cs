namespace UnityServer.Player
{
    public class PlayerStats
    {
        public int Level { get; private set; }
        public int Exp { get; private set; }
        public int Hp { get; private set; }
        public int MaxHp { get; private set; }
        public int Mp { get; private set; }
        public int MaxMp { get; private set; }

        public bool IsDead => Hp <= 0;


        public PlayerStats()
        {
            Level = 1;
            Exp = 0;
            MaxHp = DataManager.GetMaxHp(Level);
            MaxMp = DataManager.GetMaxMp(Level);
            Hp = MaxHp;
            Mp = MaxMp;
        }

        public PlayerStats(int level, int exp, int hp, int mp)
        {
            Level = level;
            Exp = exp;
            MaxHp = DataManager.GetMaxHp(level);
            MaxMp = DataManager.GetMaxMp(level);
            Hp = Math.Min(hp, MaxHp);
            Mp = Math.Min(mp, MaxMp);
        }

        public void TakeDamage(int damage)
        {
            Hp = Math.Max(0, Hp - damage);
        }

        public void Heal(int amount)
        {
            Hp = Math.Min(MaxHp, Hp + amount);
        }

        public void UseMp(int amount)
        {
            Mp = Math.Max(0, Mp - amount);
        }

        public void RecoverMp(int amount)
        {
            Mp = Math.Min(MaxMp, Mp + amount);
        }

        public bool AddExp(int amount)
        {
            if (Level >= Constants.MaxLevel)
                return false;

            Exp += amount;

            int newLevel = DataManager.CalculateLevel(Exp);
            if (newLevel > Level)
            {
                LevelUp(newLevel);
                return true;
            }

            return false;
        }

        private void LevelUp(int newLevel)
        {
            int oldMaxHp = MaxHp;
            int oldMaxMp = MaxMp;

            Level = newLevel;
            MaxHp = DataManager.GetMaxHp(Level);
            MaxMp = DataManager.GetMaxMp(Level);

            Hp += MaxHp - oldMaxHp;
            Mp += MaxMp - oldMaxMp;
        }

        public void Respawn()
        {
            Hp = MaxHp;
            Mp = MaxMp;
        }

        public void Load(int level, int exp, int hp, int mp)
        {
            Level = level;
            Exp = exp;
            MaxHp = DataManager.GetMaxHp(level);
            MaxMp = DataManager.GetMaxMp(level);
            Hp = Math.Min(hp, MaxHp);
            Mp = Math.Min(mp, MaxMp);
        }

        public int GetExpToNextLevel()
        {
            if (Level >= Constants.MaxLevel)
                return 0;

            int nextLevelExp = DataManager.GetRequiredExp(Level + 1);
            return nextLevelExp - Exp;
        }
    }
}

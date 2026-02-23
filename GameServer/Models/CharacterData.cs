namespace UnityServer.Models
{
    public struct CharacterData
    {
        public int Level { get; }
        public int Exp { get; }
        public int Hp { get; }
        public int Mp { get; }
        public int Map { get; }

        public CharacterData(int level, int exp, int hp, int mp, int map)
        {
            Level = level;
            Exp = exp;
            Hp = hp;
            Mp = mp;
            Map = map;
        }
    }
}

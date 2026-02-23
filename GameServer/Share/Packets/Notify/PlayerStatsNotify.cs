using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class PlayerStatsNotify : Packet
    {
        public override PacketType Type => PacketType.PlayerStatsNotify;

        public int Level { get; private set; }
        public int Exp { get; private set; }
        public int RequiredExp { get; private set; }
        public int Hp { get; private set; }
        public int MaxHp { get; private set; }
        public int Mp { get; private set; }
        public int MaxMp { get; private set; }

        public PlayerStatsNotify() { }
        public PlayerStatsNotify(int level, int exp, int requiredExp, int hp, int maxHp, int mp, int maxMp)
        {
            Level = level;
            Exp = exp;
            RequiredExp = requiredExp;
            Hp = hp;
            MaxHp = maxHp;
            Mp = mp;
            MaxMp = maxMp;
        }

        public override void Deserialize(BinaryReader reader)
        {
            Level = reader.ReadInt32();
            Exp = reader.ReadInt32();
            RequiredExp = reader.ReadInt32();
            Hp = reader.ReadInt32();
            MaxHp = reader.ReadInt32();
            Mp = reader.ReadInt32();
            MaxMp = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(Level);
            writer.Write(Exp);
            writer.Write(RequiredExp);
            writer.Write(Hp);
            writer.Write(MaxHp);
            writer.Write(Mp);
            writer.Write(MaxMp);
        }
    }
}

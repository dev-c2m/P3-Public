using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class MonsterDamageNotify : Packet
    {
        public override PacketType Type => PacketType.MonsterDamageNotify;

        public int AttackerId { get; private set; }
        public int UniqueId { get; private set; }
        public int Damage { get; private set; }
        public int CurrentHp { get; private set; }
        public int MaxHp { get; private set; }

        public MonsterDamageNotify() { }
        public MonsterDamageNotify(int attackerId, int uniqueId, int damage, int currentHp, int maxHp)
        {
            AttackerId = attackerId;
            UniqueId = uniqueId;
            Damage = damage;
            CurrentHp = currentHp;
            MaxHp = maxHp;
        }

        public override void Deserialize(BinaryReader reader)
        {
            AttackerId = reader.ReadInt32();
            UniqueId = reader.ReadInt32();
            Damage = reader.ReadInt32();
            CurrentHp = reader.ReadInt32();
            MaxHp = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(AttackerId);
            writer.Write(UniqueId);
            writer.Write(Damage);
            writer.Write(CurrentHp);
            writer.Write(MaxHp);
        }
    }
}

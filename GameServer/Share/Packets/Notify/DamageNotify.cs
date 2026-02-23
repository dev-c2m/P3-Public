using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class DamageNotify : Packet
    {
        public override PacketType Type => PacketType.DamageNotify;

        public int AttackerId { get; private set; }
        public int TargetId { get; private set; }
        public int Damage { get; private set; }
        public int TargetHp { get; private set; }
        public int TargetMaxHp { get; private set; }

        public DamageNotify() { }
        public DamageNotify(int attackerId, int targetId, int damage, int targetHp, int targetMaxHp)
        {
            AttackerId = attackerId;
            TargetId = targetId;
            Damage = damage;
            TargetHp = targetHp;
            TargetMaxHp = targetMaxHp;
        }

        public override void Deserialize(BinaryReader reader)
        {
            AttackerId = reader.ReadInt32();
            TargetId = reader.ReadInt32();
            Damage = reader.ReadInt32();
            TargetHp = reader.ReadInt32();
            TargetMaxHp = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(AttackerId);
            writer.Write(TargetId);
            writer.Write(Damage);
            writer.Write(TargetHp);
            writer.Write(TargetMaxHp);
        }
    }
}

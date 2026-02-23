using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class HpChangeNotify : Packet
    {
        public override PacketType Type => PacketType.HpChangeNotify;

        public int Hp { get; private set; }
        public int MaxHp { get; private set; }

        public HpChangeNotify() { }
        public HpChangeNotify(int hp, int maxHp)
        {
            Hp = hp;
            MaxHp = maxHp;
        }

        public override void Deserialize(BinaryReader reader)
        {
            Hp = reader.ReadInt32();
            MaxHp = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(Hp);
            writer.Write(MaxHp);
        }
    }
}

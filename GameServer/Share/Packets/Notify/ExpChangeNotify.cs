using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class ExpChangeNotify : Packet
    {
        public override PacketType Type => PacketType.ExpChangeNotify;

        public int Exp { get; private set; }
        public int RequiredExp { get; private set; }

        public ExpChangeNotify() { }
        public ExpChangeNotify(int exp, int requiredExp)
        {
            Exp = exp;
            RequiredExp = requiredExp;
        }

        public override void Deserialize(BinaryReader reader)
        {
            Exp = reader.ReadInt32();
            RequiredExp = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(Exp);
            writer.Write(RequiredExp);
        }
    }
}

using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class MpChangeNotify : Packet
    {
        public override PacketType Type => PacketType.MpChangeNotify;

        public int Mp { get; private set; }
        public int MaxMp { get; private set; }

        public MpChangeNotify() { }
        public MpChangeNotify(int mp, int maxMp)
        {
            Mp = mp;
            MaxMp = maxMp;
        }

        public override void Deserialize(BinaryReader reader)
        {
            Mp = reader.ReadInt32();
            MaxMp = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(Mp);
            writer.Write(MaxMp);
        }
    }
}

using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class EnterCompleteNotify : Packet
    {
        public override PacketType Type => PacketType.EnterCompleteNotify;

        public long ServerTime { get; private set; }

        public EnterCompleteNotify() { }
        public EnterCompleteNotify(long serverTime)
        {
            ServerTime = serverTime;
        }

        public override void Deserialize(BinaryReader reader)
        {
            ServerTime = reader.ReadInt64();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(ServerTime);
        }
    }
}

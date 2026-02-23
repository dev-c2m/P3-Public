using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class MonsterDeathNotify : Packet
    {
        public override PacketType Type => PacketType.MonsterDeathNotify;

        public int UniqueId { get; private set; }

        public MonsterDeathNotify() { }
        public MonsterDeathNotify(int uniqueId)
        {
            UniqueId = uniqueId;
        }

        public override void Deserialize(BinaryReader reader)
        {
            UniqueId = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(UniqueId);
        }
    }
}

using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class MonsterSpawnNotify : Packet
    {
        public override PacketType Type => PacketType.MonsterSpawnNotify;

        public int MapId { get; private set; }
        public int MonsterId { get; private set; }
        public int UniqueId { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public MonsterSpawnNotify() { }
        public MonsterSpawnNotify(int mapId, int monsterId, int uniqueId, float x, float y, float z)
        {
            MapId = mapId;
            MonsterId = monsterId;
            UniqueId = uniqueId;
            X = x;
            Y = y;
            Z = z;
        }

        public override void Deserialize(BinaryReader reader)
        {
            MapId = reader.ReadInt32();
            MonsterId = reader.ReadInt32();
            UniqueId = reader.ReadInt32();
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(MapId);
            writer.Write(MonsterId);
            writer.Write(UniqueId);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }
    }
}

using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class RespawnNotify : Packet
    {
        public override PacketType Type => PacketType.RespawnNotify;

        public int PlayerId { get; private set; }
        public string Nickname { get; private set; } = string.Empty;
        public int MapId { get; private set; }
        public float PositionX { get; private set; }
        public float PositionY { get; private set; }
        public float PositionZ { get; private set; }
        public float RotationY { get; private set; }

        public RespawnNotify() { }
        public RespawnNotify(int playerId, string nickname, int mapId, float x, float y, float z, float rotationY)
        {
            PlayerId = playerId;
            Nickname = nickname;
            MapId = mapId;
            PositionX = x;
            PositionY = y;
            PositionZ = z;
            RotationY = rotationY;
        }

        public override void Deserialize(BinaryReader reader)
        {
            PlayerId = reader.ReadInt32();
            Nickname = reader.ReadString();
            MapId = reader.ReadInt32();
            PositionX = reader.ReadSingle();
            PositionY = reader.ReadSingle();
            PositionZ = reader.ReadSingle();
            RotationY = reader.ReadSingle();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(PlayerId);
            writer.Write(Nickname);
            writer.Write(MapId);
            writer.Write(PositionX);
            writer.Write(PositionY);
            writer.Write(PositionZ);
            writer.Write(RotationY);
        }
    }
}

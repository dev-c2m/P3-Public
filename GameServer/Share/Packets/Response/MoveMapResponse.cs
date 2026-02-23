using System.IO;

namespace UnityServer.Share.Packets.Response
{
    class MoveMapResponse : Packet
    {
        public override PacketType Type => PacketType.MoveMapResponse;

        public bool IsSuccess { get; private set; }
        public int MapId { get; private set; }
        public float SpawnX { get; private set; }
        public float SpawnY { get; private set; }
        public float SpawnZ { get; private set; }
        public string Message { get; private set; } = string.Empty;

        public MoveMapResponse() { }
        public MoveMapResponse(bool isSuccess, int mapId, float spawnX, float spawnY, float spawnZ, string message = "")
        {
            IsSuccess = isSuccess;
            MapId = mapId;
            SpawnX = spawnX;
            SpawnY = spawnY;
            SpawnZ = spawnZ;
            Message = message;
        }

        public override void Deserialize(BinaryReader reader)
        {
            IsSuccess = reader.ReadBoolean();
            MapId = reader.ReadInt32();
            SpawnX = reader.ReadSingle();
            SpawnY = reader.ReadSingle();
            SpawnZ = reader.ReadSingle();
            Message = reader.ReadString();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(IsSuccess);
            writer.Write(MapId);
            writer.Write(SpawnX);
            writer.Write(SpawnY);
            writer.Write(SpawnZ);
            writer.Write(Message);
        }
    }
}

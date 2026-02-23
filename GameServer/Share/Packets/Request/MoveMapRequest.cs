using System.IO;

namespace UnityServer.Share.Packets.Request
{
    class MoveMapRequest : Packet
    {
        public override PacketType Type => PacketType.MoveMapRequest;

        public int MapId { get; private set; }

        public MoveMapRequest() { }
        public MoveMapRequest(int mapId)
        {
            MapId = mapId;
        }

        public override void Deserialize(BinaryReader reader)
        {
            MapId = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(MapId);
        }
    }
}

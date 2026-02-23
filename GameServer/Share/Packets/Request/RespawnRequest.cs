using System.IO;

namespace UnityServer.Share.Packets.Request
{
    class RespawnRequest : Packet
    {
        public override PacketType Type => PacketType.RespawnRequest;

        public RespawnRequest() { }

        public override void Deserialize(BinaryReader reader)
        {
        }

        protected override void Serialize(BinaryWriter writer)
        {
        }
    }
}

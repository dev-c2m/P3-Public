using System.IO;

namespace UnityServer.Share.Packets.Request
{
    class PlayerLeaveRequest : Packet
    {
        public override PacketType Type => PacketType.PlayerLeaveRequest;

        public PlayerLeaveRequest() { }

        public override void Deserialize(BinaryReader reader)
        {
        }

        protected override void Serialize(BinaryWriter writer)
        {
        }
    }
}

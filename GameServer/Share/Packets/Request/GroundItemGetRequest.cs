using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Share.Packets.Request
{
    class GroundItemGetRequest : Packet
    {
        public override PacketType Type => PacketType.GroundItemGetRequest;

        public int GroundItemId { get; private set; }

        public GroundItemGetRequest() { }
        public GroundItemGetRequest(int groundItemId)
        {
            GroundItemId = groundItemId;
        }

        public override void Deserialize(BinaryReader reader)
        {
            GroundItemId = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(GroundItemId);
        }
    }
}

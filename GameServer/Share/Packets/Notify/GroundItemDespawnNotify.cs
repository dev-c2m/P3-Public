using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Share.Packets.Notify
{
    class GroundItemDespawnNotify : Packet
    {
        public override PacketType Type => PacketType.GroundItemDespawnNotify;

        public int GroundItemId { get; private set; }

        public GroundItemDespawnNotify() { }
        public GroundItemDespawnNotify(int groundItemId)
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

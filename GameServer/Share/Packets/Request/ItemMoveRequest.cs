using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Share.Packets.Request
{
    class ItemMoveRequest : Packet
    {
        public override PacketType Type => PacketType.ItemMoveRequest;

        public int prevSlotIndex { get; private set; }
        public int newSlotIndex { get; private set; }

        public ItemMoveRequest() { }
        public ItemMoveRequest(int prevSlotIndex, int newSlotIndex)
        {
            this.prevSlotIndex = prevSlotIndex;
            this.newSlotIndex = newSlotIndex;
        }

        public override void Deserialize(BinaryReader reader)
        {
            prevSlotIndex = reader.ReadInt32();
            newSlotIndex = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(prevSlotIndex);
            writer.Write(newSlotIndex);
        }
    }
}

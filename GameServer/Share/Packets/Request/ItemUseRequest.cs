using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Share.Packets.Request
{
    class ItemUseRequest : Packet
    {
        public override PacketType Type => PacketType.ItemUseRequest;

        public int ItemId { get; private set; }
        public int Quantity { get; private set; }
        public int SlotIndex { get; private set; }

        public ItemUseRequest() { }
        public ItemUseRequest(int itemId, int quantity, int slotIndex)
        {
            ItemId = itemId;
            Quantity = quantity;
            SlotIndex = slotIndex;
        }


        public override void Deserialize(BinaryReader reader)
        {
            ItemId = reader.ReadInt32();
            Quantity = reader.ReadInt32();
            SlotIndex = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(ItemId);
            writer.Write(Quantity);
            writer.Write(SlotIndex);
        }
    }
}

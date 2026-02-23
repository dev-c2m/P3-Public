using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Share.Packets.Response
{
    class GroundItemGetResponse : Packet
    {
        public override PacketType Type => PacketType.GroundItemGetResponse;

        public bool IsSuccess { get; private set; }
        public int ItemId { get; private set; }
        public int Quantity { get; private set; }
        public int SlotIndex { get; private set; }
        public string Message { get; private set; } = string.Empty;

        public GroundItemGetResponse() { }
        public GroundItemGetResponse(bool isSuccess, int itemId, int quantity, int slotIndex, string message = "")
        {
            IsSuccess = isSuccess;
            ItemId = itemId;
            Quantity = quantity;
            SlotIndex = slotIndex;
            Message = message;
        }

        public override void Deserialize(BinaryReader reader)
        {
            IsSuccess = reader.ReadBoolean();
            ItemId = reader.ReadInt32();
            Quantity = reader.ReadInt32();
            SlotIndex = reader.ReadInt32();
            Message = reader.ReadString();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(IsSuccess);
            writer.Write(ItemId);
            writer.Write(Quantity);
            writer.Write(SlotIndex);
            writer.Write(Message);
        }
    }
}

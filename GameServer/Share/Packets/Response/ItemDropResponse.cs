using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Share.Packets.Response
{
    class ItemDropResponse : Packet
    {
        public override PacketType Type => PacketType.ItemDropResponse;

        public bool IsSuccess { get; private set; }
        public int SlotIndex { get; private set; }
        public int RemainingQuantity { get; private set; }
        public string Message { get; private set; } = string.Empty;

        public ItemDropResponse() { }
        public ItemDropResponse(bool isSuccess, int slotIndex, int remainingQuantity, string message = "")
        {
            IsSuccess = isSuccess;
            SlotIndex = slotIndex;
            RemainingQuantity = remainingQuantity;
            Message = message;
        }

        public override void Deserialize(BinaryReader reader)
        {
            IsSuccess = reader.ReadBoolean();
            SlotIndex = reader.ReadInt32();
            RemainingQuantity = reader.ReadInt32();
            Message = reader.ReadString();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(IsSuccess);
            writer.Write(SlotIndex);
            writer.Write(RemainingQuantity);
            writer.Write(Message);
        }
    }
}

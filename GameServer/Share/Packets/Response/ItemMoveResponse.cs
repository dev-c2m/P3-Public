using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Share.Packets.Response
{
    class ItemMoveResponse : Packet
    {
        public override PacketType Type => PacketType.ItemMoveResponse;

        public bool IsSuccess { get; private set; }
        public int PrevSlotIndex { get; private set; }
        public int NewSlotIndex { get; private set; }
        public string Message { get; private set; } = string.Empty;

        public ItemMoveResponse() { }
        public ItemMoveResponse(bool isSuccess, int prevSlotIndex, int newSlotIndex, string message = "")
        {
            IsSuccess = isSuccess;
            PrevSlotIndex = prevSlotIndex;
            NewSlotIndex = newSlotIndex;
            Message = message;
        }

        public override void Deserialize(BinaryReader reader)
        {
            IsSuccess = reader.ReadBoolean();
            PrevSlotIndex = reader.ReadInt32();
            NewSlotIndex = reader.ReadInt32();
            Message = reader.ReadString();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(IsSuccess);
            writer.Write(PrevSlotIndex);
            writer.Write(NewSlotIndex);
            writer.Write(Message);
        }
    }
}

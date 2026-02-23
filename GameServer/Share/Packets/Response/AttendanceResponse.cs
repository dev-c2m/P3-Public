using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UnityServer.Share.Packets.Response
{
    class AttendanceResponse : Packet
    {
        public override PacketType Type => PacketType.AttendanceResponse;

        public bool IsSuccess { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public int AttendanceId { get; private set; }
        public int DayCount { get; private set; }
        public int ItemId {get; private set; }
        public int ItemCount { get; private set; }

        public AttendanceResponse() { }
        public AttendanceResponse(string message)
        {
            IsSuccess = false;
            Message = message;
            AttendanceId = -1;
            DayCount = -1;
            ItemId = -1;
            ItemCount = -1;
        }
        public AttendanceResponse(int attendanceId, int dayCount, int itemId, int itemCount)
        {
            IsSuccess = true;
            Message = string.Empty;
            AttendanceId = attendanceId;
            DayCount = dayCount;
            ItemId = itemId;
            ItemCount = itemCount;
        }

        public override void Deserialize(BinaryReader reader)
        {
            IsSuccess = reader.ReadBoolean();
            Message = reader.ReadString();
            AttendanceId = reader.ReadInt32();
            DayCount = reader.ReadInt32();
            ItemId = reader.ReadInt32();
            ItemCount = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(IsSuccess);
            writer.Write(Message);
            writer.Write(AttendanceId);
            writer.Write(DayCount);
            writer.Write(ItemId);
            writer.Write(ItemCount);
        }
    }
}

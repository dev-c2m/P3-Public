using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class AttendanceNotify : Packet
    {
        public override PacketType Type => PacketType.AttendanceNotify;

        public int AttendanceId { get; private set; }
        public int DayCount { get; private set; }
        public bool CanCheckIn { get; private set; }

        public AttendanceNotify() { }
        public AttendanceNotify(int attendanceId, int dayCount, bool CanCheckIn)
        {
            AttendanceId = attendanceId;
            DayCount = dayCount;
            this.CanCheckIn = CanCheckIn;
        }

        public override void Deserialize(BinaryReader reader)
        {
            AttendanceId = reader.ReadInt32();
            DayCount = reader.ReadInt32();
            CanCheckIn = reader.ReadBoolean();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(AttendanceId);
            writer.Write(DayCount);
            writer.Write(CanCheckIn);
        }
    }
}

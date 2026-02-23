using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UnityServer.Share.Packets.Request
{
    class AttendanceRequest : Packet
    {
        public override PacketType Type => PacketType.AttendanceRequest;

        public int AttendanceId { get; private set; }

        public AttendanceRequest() { }
        public AttendanceRequest(int attendanceId)
        {
            AttendanceId = attendanceId;
        }

        public override void Deserialize(BinaryReader reader)
        {
            AttendanceId = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(AttendanceId);
        }
    }
}

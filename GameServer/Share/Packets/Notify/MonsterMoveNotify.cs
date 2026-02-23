using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class MonsterMoveNotify : Packet
    {
        public override PacketType Type => PacketType.MonsterMoveNotify;

        public int UniqueId { get; private set; }
        public float StartX { get; private set; }
        public float StartY { get; private set; }
        public float StartZ { get; private set; }
        public float TargetX { get; private set; }
        public float TargetY { get; private set; }
        public float TargetZ { get; private set; }
        public long StartTime { get; private set; }
        public long ArriveTime { get; private set; }

        public MonsterMoveNotify() { }
        public MonsterMoveNotify(int uniqueId, float startX, float startY, float startZ, float targetX, float targetY, float targetZ, long startTime, long arriveTime)
        {
            UniqueId = uniqueId;
            StartX = startX;
            StartY = startY;
            StartZ = startZ;
            TargetX = targetX;
            TargetY = targetY;
            TargetZ = targetZ;
            StartTime = startTime;
            ArriveTime = arriveTime;
        }

        public override void Deserialize(BinaryReader reader)
        {
            UniqueId = reader.ReadInt32();
            StartX = reader.ReadSingle();
            StartY = reader.ReadSingle();
            StartZ = reader.ReadSingle();
            TargetX = reader.ReadSingle();
            TargetY = reader.ReadSingle();
            TargetZ = reader.ReadSingle();
            StartTime = reader.ReadInt64();
            ArriveTime = reader.ReadInt64();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(UniqueId);
            writer.Write(StartX);
            writer.Write(StartY);
            writer.Write(StartZ);
            writer.Write(TargetX);
            writer.Write(TargetY);
            writer.Write(TargetZ);
            writer.Write(StartTime);
            writer.Write(ArriveTime);
        }
    }
}

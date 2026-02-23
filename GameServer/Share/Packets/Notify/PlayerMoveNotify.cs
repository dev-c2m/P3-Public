using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class PlayerMoveNotify : Packet
    {
        public override PacketType Type => PacketType.PlayerMoveNotify;

        public int PlayerId { get; private set; }
        public float PositionX { get; private set; }
        public float PositionY { get; private set; }
        public float PositionZ { get; private set; }
        public float RotationY { get; private set; }
        public float DirectionX { get; private set; }
        public float DirectionZ { get; private set; }
        public bool IsMoving { get; private set; }
        public float Speed { get; private set; }

        public PlayerMoveNotify() { }
        public PlayerMoveNotify(int playerId, float x, float y, float z, float rotationY,
            float dirX, float dirZ, bool isMoving, float speed)
        {
            PlayerId = playerId;
            PositionX = x;
            PositionY = y;
            PositionZ = z;
            RotationY = rotationY;
            DirectionX = dirX;
            DirectionZ = dirZ;
            IsMoving = isMoving;
            Speed = speed;
        }

        public override void Deserialize(BinaryReader reader)
        {
            PlayerId = reader.ReadInt32();
            PositionX = reader.ReadSingle();
            PositionY = reader.ReadSingle();
            PositionZ = reader.ReadSingle();
            RotationY = reader.ReadSingle();
            DirectionX = reader.ReadSingle();
            DirectionZ = reader.ReadSingle();
            IsMoving = reader.ReadBoolean();
            Speed = reader.ReadSingle();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(PlayerId);
            writer.Write(PositionX);
            writer.Write(PositionY);
            writer.Write(PositionZ);
            writer.Write(RotationY);
            writer.Write(DirectionX);
            writer.Write(DirectionZ);
            writer.Write(IsMoving);
            writer.Write(Speed);
        }
    }
}

using System.IO;

namespace UnityServer.Share.Packets.Request
{
    class PlayerMoveRequest : Packet
    {
        public override PacketType Type => PacketType.PlayerMoveRequest;

        public float DirectionX { get; private set; }
        public float DirectionZ { get; private set; }
        public bool IsMoving { get; private set; }
        public float RotationY { get; private set; }

        public PlayerMoveRequest() { }
        public PlayerMoveRequest(float dirX, float dirZ, bool isMoving, float rotationY)
        {
            DirectionX = dirX;
            DirectionZ = dirZ;
            IsMoving = isMoving;
            RotationY = rotationY;
        }

        public override void Deserialize(BinaryReader reader)
        {
            DirectionX = reader.ReadSingle();
            DirectionZ = reader.ReadSingle();
            IsMoving = reader.ReadBoolean();
            RotationY = reader.ReadSingle();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(DirectionX);
            writer.Write(DirectionZ);
            writer.Write(IsMoving);
            writer.Write(RotationY);
        }
    }
}

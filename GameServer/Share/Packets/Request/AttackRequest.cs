using System.IO;

namespace UnityServer.Share.Packets.Request
{
    class AttackRequest : Packet
    {
        public override PacketType Type => PacketType.AttackRequest;

        public int SkillId { get; private set; }
        public float DirectionX { get; private set; }
        public float DirectionY { get; private set; }
        public float DirectionZ { get; private set; }

        public AttackRequest() { }
        public AttackRequest(int skillId, float dirX, float dirY, float dirZ)
        {
            SkillId = skillId;
            DirectionX = dirX;
            DirectionY = dirY;
            DirectionZ = dirZ;
        }

        public override void Deserialize(BinaryReader reader)
        {
            SkillId = reader.ReadInt32();
            DirectionX = reader.ReadSingle();
            DirectionY = reader.ReadSingle();
            DirectionZ = reader.ReadSingle();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(SkillId);
            writer.Write(DirectionX);
            writer.Write(DirectionY);
            writer.Write(DirectionZ);
        }
    }
}

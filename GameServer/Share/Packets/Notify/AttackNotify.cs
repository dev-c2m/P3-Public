using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class AttackNotify : Packet
    {
        public override PacketType Type => PacketType.AttackNotify;

        public int PlayerId { get; private set; }
        public int SkillId { get; private set; }
        public float DirectionX { get; private set; }
        public float DirectionY { get; private set; }
        public float DirectionZ { get; private set; }
        public float PositionX { get; private set; }
        public float PositionY { get; private set; }
        public float PositionZ { get; private set; }

        public AttackNotify() { }
        public AttackNotify(int playerId, int skillId, float dirX, float dirY, float dirZ, float posX, float posY, float posZ)
        {
            PlayerId = playerId;
            SkillId = skillId;
            DirectionX = dirX;
            DirectionY = dirY;
            DirectionZ = dirZ;
            PositionX = posX;
            PositionY = posY;
            PositionZ = posZ;
        }

        public override void Deserialize(BinaryReader reader)
        {
            PlayerId = reader.ReadInt32();
            SkillId = reader.ReadInt32();
            DirectionX = reader.ReadSingle();
            DirectionY = reader.ReadSingle();
            DirectionZ = reader.ReadSingle();
            PositionX = reader.ReadSingle();
            PositionY = reader.ReadSingle();
            PositionZ = reader.ReadSingle();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(PlayerId);
            writer.Write(SkillId);
            writer.Write(DirectionX);
            writer.Write(DirectionY);
            writer.Write(DirectionZ);
            writer.Write(PositionX);
            writer.Write(PositionY);
            writer.Write(PositionZ);
        }
    }
}

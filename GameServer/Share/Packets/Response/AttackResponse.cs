using System.IO;

namespace UnityServer.Share.Packets.Response
{
    class AttackResponse : Packet
    {
        public override PacketType Type => PacketType.AttackResponse;

        public bool IsSuccess { get; private set; }
        public int SkillId { get; private set; }
        public string Message { get; private set; } = string.Empty;

        public AttackResponse() { }
        public AttackResponse(bool isSuccess, int skillId, string message = "")
        {
            IsSuccess = isSuccess;
            SkillId = skillId;
            Message = message;
        }

        public override void Deserialize(BinaryReader reader)
        {
            IsSuccess = reader.ReadBoolean();
            SkillId = reader.ReadInt32();
            Message = reader.ReadString();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(IsSuccess);
            writer.Write(SkillId);
            writer.Write(Message);
        }
    }
}

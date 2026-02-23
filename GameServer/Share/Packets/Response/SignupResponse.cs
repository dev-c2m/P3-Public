using System.IO;

namespace UnityServer.Share.Packets.Response
{
    class SignupResponse : Packet
    {
        public override PacketType Type => PacketType.SignupResponse;

        public bool IsSuccess { get; private set; }
        public string Message { get; private set; } = string.Empty;

        public SignupResponse() { }
        public SignupResponse(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public override void Deserialize(BinaryReader reader)
        {
            IsSuccess = reader.ReadBoolean();
            Message = reader.ReadString();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(IsSuccess);
            writer.Write(Message);
        }
    }
}

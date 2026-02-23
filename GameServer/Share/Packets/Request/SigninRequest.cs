using System.IO;

namespace UnityServer.Share.Packets.Request
{
    class SigninRequest : Packet
    {
        public override PacketType Type => PacketType.SigninRequest;

        public string LoginId { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;

        public SigninRequest() { }
        public SigninRequest(string loginId, string password)
        {
            LoginId = loginId;
            Password = password;
        }

        public override void Deserialize(BinaryReader reader)
        {
            LoginId = reader.ReadString();
            Password = reader.ReadString();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(LoginId);
            writer.Write(Password);
        }
    }
}

using System.IO;

namespace UnityServer.Share.Packets.Request
{
    class SignupRequest : Packet
    {
        public override PacketType Type => PacketType.SignupRequest;

        public string LoginId { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public string Nickname { get; private set; } = string.Empty;

        public SignupRequest() { }
        public SignupRequest(string loginId, string password, string nickname)
        {
            LoginId = loginId;
            Password = password;
            Nickname = nickname;
        }

        public override void Deserialize(BinaryReader reader)
        {
            LoginId = reader.ReadString();
            Password = reader.ReadString();
            Nickname = reader.ReadString();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(LoginId);
            writer.Write(Password);
            writer.Write(Nickname);
        }
    }
}

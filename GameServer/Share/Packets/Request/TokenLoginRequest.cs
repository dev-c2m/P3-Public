using System.IO;

namespace UnityServer.Share.Packets.Request
{
    class TokenLoginRequest : Packet
    {
        public override PacketType Type => PacketType.TokenLoginRequest;

        public string Token { get; private set; } = string.Empty;

        public TokenLoginRequest() { }
        public TokenLoginRequest(string token)
        {
            Token = token;
        }

        public override void Deserialize(BinaryReader reader)
        {
            Token = reader.ReadString();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(Token);
        }
    }
}

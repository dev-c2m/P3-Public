using System.IO;

namespace UnityServer.Share.Packets.Request
{
    class ChatRequest : Packet
    {
        public override PacketType Type => PacketType.ChatRequest;

        public string Message { get; private set; } = string.Empty;

        public ChatRequest() { }
        public ChatRequest(string message)
        {
            Message = message;
        }

        public override void Deserialize(BinaryReader reader)
        {
            Message = reader.ReadString();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(Message);
        }
    }
}

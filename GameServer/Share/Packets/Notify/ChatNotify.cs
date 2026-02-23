using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class ChatNotify : Packet
    {
        public override PacketType Type => PacketType.ChatNotify;

        public string Nickname { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;

        public ChatNotify() { }
        public ChatNotify(string nickname, string message)
        {
            Nickname = nickname;
            Message = message;
        }

        public override void Deserialize(BinaryReader reader)
        {
            Nickname = reader.ReadString();
            Message = reader.ReadString();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(Nickname);
            writer.Write(Message);
        }
    }
}

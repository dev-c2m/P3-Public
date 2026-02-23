using System.IO;

namespace UnityServer.Share.Packets.Response
{
    class SigninResponse : Packet
    {
        public override PacketType Type => PacketType.SigninResponse;

        public bool IsSuccess { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public string Nickname { get; private set; } = string.Empty;
        public int MapId { get; private set; }

        public SigninResponse() { }
        public SigninResponse(bool isSuccess, string message, string nickname = "", int mapId = 0)
        {
            IsSuccess = isSuccess;
            Message = message;
            Nickname = nickname;
            MapId = mapId;
        }

        public override void Deserialize(BinaryReader reader)
        {
            IsSuccess = reader.ReadBoolean();
            Message = reader.ReadString();
            Nickname = reader.ReadString();
            MapId = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(IsSuccess);
            writer.Write(Message);
            writer.Write(Nickname);
            writer.Write(MapId);
        }
    }
}

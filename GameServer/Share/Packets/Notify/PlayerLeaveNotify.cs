using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class PlayerLeaveNotify : Packet
    {
        public override PacketType Type => PacketType.PlayerLeaveNotify;

        public int PlayerId { get; private set; }

        public PlayerLeaveNotify() { }
        public PlayerLeaveNotify(int playerId)
        {
            PlayerId = playerId;
        }

        public override void Deserialize(BinaryReader reader)
        {
            PlayerId = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(PlayerId);
        }
    }
}

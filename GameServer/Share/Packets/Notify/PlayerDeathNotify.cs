using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class PlayerDeathNotify : Packet
    {
        public override PacketType Type => PacketType.PlayerDeathNotify;

        public int PlayerId { get; private set; }

        public PlayerDeathNotify() { }
        public PlayerDeathNotify(int playerId)
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

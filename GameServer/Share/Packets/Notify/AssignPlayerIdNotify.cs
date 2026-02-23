using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class AssignPlayerIdNotify : Packet
    {
        public override PacketType Type => PacketType.AssignPlayerIdNotify;

        public int PlayerId { get; private set; }

        public AssignPlayerIdNotify() { }
        public AssignPlayerIdNotify(int playerId)
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

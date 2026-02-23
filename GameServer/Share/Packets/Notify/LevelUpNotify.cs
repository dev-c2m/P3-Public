using System.IO;

namespace UnityServer.Share.Packets.Notify
{
    class LevelUpNotify : Packet
    {
        public override PacketType Type => PacketType.LevelUpNotify;

        public int PlayerId { get; private set; }
        public int Level { get; private set; }

        public LevelUpNotify() { }
        public LevelUpNotify(int playerId, int level)
        {
            PlayerId = playerId;
            Level = level;
        }

        public override void Deserialize(BinaryReader reader)
        {
            PlayerId = reader.ReadInt32();
            Level = reader.ReadInt32();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(PlayerId);
            writer.Write(Level);
        }
    }
}

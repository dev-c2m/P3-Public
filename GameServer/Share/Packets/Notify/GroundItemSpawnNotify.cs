using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Share.Packets.Notify
{
    class GroundItemSpawnNotify : Packet
    {
        public override PacketType Type => PacketType.GroundItemSpawnNotify;

        public int GroundItemId { get; private set; }
        public int ItemId { get; private set; }
        public int Quantity { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public GroundItemSpawnNotify() { }
        public GroundItemSpawnNotify(int groundItemId, int itemId, int quantity, float x, float y, float z)
        {
            GroundItemId = groundItemId;
            ItemId = itemId;
            Quantity = quantity;
            X = x;
            Y = y;
            Z = z;
        }

        public override void Deserialize(BinaryReader reader)
        {
            GroundItemId = reader.ReadInt32();
            ItemId = reader.ReadInt32();
            Quantity = reader.ReadInt32();
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(GroundItemId);
            writer.Write(ItemId);
            writer.Write(Quantity);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }
    }
}

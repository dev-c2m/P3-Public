using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Share.Packets
{
    abstract class Packet
    {
        public abstract PacketType Type { get; }

        protected abstract void Serialize(BinaryWriter writer);
        public abstract void Deserialize(BinaryReader reader);

        public byte[] ToBytes()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            writer.Write(0);
            writer.Write((int)Type);
            Serialize(writer);
            ms.Position = 0;
            writer.Write((int)ms.Length);

            return ms.ToArray();
        }
    }
}

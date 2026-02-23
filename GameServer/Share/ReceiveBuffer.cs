using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Share
{
    class ReceiveBuffer
    {
        private const int MAX_BUFFER_SIZE = 8192;

        private byte[] buffer = new byte[MAX_BUFFER_SIZE];
        private int writePos = 0;
        private int readPos = 0;

        public int DataSize => writePos - readPos;

        public void Write(byte[] data, int bytes)
        {
            if (buffer.Length - writePos < bytes)
            {
                Compact();
            }

            Array.Copy(data, 0, buffer, writePos, bytes);
            writePos += bytes;
        }

        public bool TryReadPacket(out byte[] packet)
        {
            packet = null;

            if (DataSize < 4)
                return false;

            int packetSize = BitConverter.ToInt32(buffer, readPos);

            if (DataSize < packetSize || packetSize > MAX_BUFFER_SIZE)
                return false;

            packet = new byte[packetSize];
            Array.Copy(buffer, readPos, packet, 0, packetSize);

            readPos += packetSize;

            if (readPos == writePos)
            {
                readPos = 0;
                writePos = 0;
            }

            return true;
        }

        private void Compact()
        {
            int dataSize = writePos - readPos;
            Array.Copy(buffer, readPos, buffer, 0, dataSize);
            readPos = 0;
            writePos = dataSize;
        }
    }

}

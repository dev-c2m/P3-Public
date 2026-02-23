using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace UnityServer.Share
{
    class SendContext
    {
        private readonly NetworkStream stream;
        private ConcurrentQueue<byte[]> sendQueue = new ConcurrentQueue<byte[]>();
        private bool isClosed = false;
        private Task? lastWriteTask = null;

        public SendContext(NetworkStream stream)
        {
            this.stream = stream;
        }

        public void Close()
        {
            isClosed = true;
            while (sendQueue.TryDequeue(out _)) { }
        }

        public void Flush()
        {
            if (isClosed || sendQueue.IsEmpty)
                return;

            if (lastWriteTask != null && !lastWriteTask.IsCompleted)
                return;

            if (!stream.CanWrite)
            {
                Close();
                return;
            }

            List<byte[]> packets = new List<byte[]>();
            while (sendQueue.TryDequeue(out byte[]? packet))
            {
                packets.Add(packet);
            }

            int totalSize = 0;
            foreach (byte[] packet in packets)
            {
                totalSize += packet.Length;
            }

            byte[] merged = new byte[totalSize];
            int offset = 0;
            foreach (byte[] packet in packets)
            {
                Buffer.BlockCopy(packet, 0, merged, offset, packet.Length);
                offset += packet.Length;
            }

            try
            {
                lastWriteTask = stream.WriteAsync(merged, 0, merged.Length);
            }
            catch (Exception)
            {
                Close();
            }
        }

        public void Enqueue(byte[] packet)
        {
            if (isClosed)
                return;

            sendQueue.Enqueue(packet);
        }
    }
}

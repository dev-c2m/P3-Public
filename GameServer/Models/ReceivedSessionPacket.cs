using System.Net.Sockets;
using UnityServer.Share.Packets;

namespace UnityServer.Models
{
    class ReceivedSessionPacket
    {
        public Session? Session { get; private set; } = null;
        public Packet? Packet { get; private set; } = null;
        public Constants.ReceivedPacketType Type { get; private set; }
        public TcpClient TcpClient { get; private set; }
        public DbResult? DbResult { get; private set; } = null;

        public ReceivedSessionPacket(Session session, Packet packet)
        {
            Session = session;
            Packet = packet;
            Type = Constants.ReceivedPacketType.Packet;
        }

        public ReceivedSessionPacket(Session session)
        {
            Session = session;
            Packet = null;
            Type = Constants.ReceivedPacketType.Leave;
        }

        public ReceivedSessionPacket(TcpClient client)
        {
            TcpClient = client;
            Session = null;
            Packet = null;
            Type = Constants.ReceivedPacketType.Create;
        }

        public ReceivedSessionPacket(DbResult dbResult)
        {
            DbResult = dbResult;
            Session = dbResult.Session;
            Type = Constants.ReceivedPacketType.DbResult;
        }
    }
}

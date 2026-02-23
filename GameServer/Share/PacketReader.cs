using System.IO;
using UnityServer.Share.Packets;
using UnityServer.Share.Packets.Request;
using UnityServer.Share.Packets.Response;
using UnityServer.Share.Packets.Notify;

namespace UnityServer.Share
{
    static class PacketReader
    {
        public static Packet? Read(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                int packetSize = reader.ReadInt32();
                PacketType packetType = (PacketType)reader.ReadInt32();

                Packet? packet = packetType switch
                {
                    PacketType.ChatRequest => new ChatRequest(),
                    PacketType.PlayerMoveRequest => new PlayerMoveRequest(),
                    PacketType.PlayerLeaveRequest => new PlayerLeaveRequest(),
                    PacketType.RespawnRequest => new RespawnRequest(),
                    PacketType.TokenLoginRequest => new TokenLoginRequest(),
                    PacketType.AttendanceRequest => new AttendanceRequest(),
                    PacketType.AttackRequest => new AttackRequest(),
                    PacketType.MoveMapRequest => new MoveMapRequest(),
                    PacketType.ItemDropRequest => new ItemDropRequest(),
                    PacketType.ItemUseRequest => new ItemUseRequest(),
                    PacketType.ItemMoveRequest => new ItemMoveRequest(),
                    PacketType.GroundItemGetRequest => new GroundItemGetRequest(),

                    PacketType.SignupResponse => new SignupResponse(),
                    PacketType.SigninResponse => new SigninResponse(),
                    PacketType.AttendanceResponse => new AttendanceResponse(),
                    PacketType.AttackResponse => new AttackResponse(),
                    PacketType.MoveMapResponse => new MoveMapResponse(),
                    PacketType.ItemDropResponse => new ItemDropResponse(),
                    PacketType.ItemUseResponse => new ItemUseResponse(),
                    PacketType.ItemMoveResponse => new ItemMoveResponse(),
                    PacketType.GroundItemGetResponse => new GroundItemGetResponse(),

                    PacketType.ChatNotify => new ChatNotify(),
                    PacketType.PlayerMoveNotify => new PlayerMoveNotify(),
                    PacketType.PlayerSpawnNotify => new PlayerSpawnNotify(),
                    PacketType.PlayerLeaveNotify => new PlayerLeaveNotify(),
                    PacketType.AssignPlayerIdNotify => new AssignPlayerIdNotify(),
                    PacketType.EnterCompleteNotify => new EnterCompleteNotify(),
                    PacketType.PlayerStatsNotify => new PlayerStatsNotify(),
                    PacketType.HpChangeNotify => new HpChangeNotify(),
                    PacketType.MpChangeNotify => new MpChangeNotify(),
                    PacketType.ExpChangeNotify => new ExpChangeNotify(),
                    PacketType.LevelUpNotify => new LevelUpNotify(),
                    PacketType.PlayerDeathNotify => new PlayerDeathNotify(),
                    PacketType.RespawnNotify => new RespawnNotify(),
                    PacketType.AttendanceNotify => new AttendanceNotify(),
                    PacketType.AttackNotify => new AttackNotify(),
                    PacketType.DamageNotify => new DamageNotify(),
                    PacketType.MonsterSpawnNotify => new MonsterSpawnNotify(),
                    PacketType.MonsterDeathNotify => new MonsterDeathNotify(),
                    PacketType.MonsterDamageNotify => new MonsterDamageNotify(),
                    PacketType.GroundItemSpawnNotify => new GroundItemSpawnNotify(),
                    PacketType.GroundItemDespawnNotify => new GroundItemDespawnNotify(),
                    PacketType.MonsterMoveNotify => new MonsterMoveNotify(),
                    PacketType.ItemAddNotify => new ItemAddNotify(),

                    _ => null
                };

                packet?.Deserialize(reader);
                return packet;
            }
        }
    }
}

using UnityServer.Models;
using UnityServer.Share.Packets.Request;
using UnityServer.Share.Packets.Response;
using UnityServer.Share.Packets.Notify;
using UnityServer.Player;
using System.Numerics;

namespace UnityServer.Share
{
    static class PacketWriter
    {
        #region Response

        public static byte[] RegisterResponse(bool isSuccess, string message)
        {
            return new SignupResponse(isSuccess, message).ToBytes();
        }

        public static byte[] LoginResponse(bool isSuccess, string message, string nickname = "", int mapId = 0)
        {
            return new SigninResponse(isSuccess, message, nickname, mapId).ToBytes();
        }

        public static byte[] AttendanceResponse(int attendanceId, int dayCount, int itemId, int quantity, bool isSuccess, string message = "")
        {
            if (isSuccess) return new AttendanceResponse(attendanceId, dayCount, itemId, quantity).ToBytes();
            else return new AttendanceResponse(message).ToBytes();
        }

        public static byte[] AttackResponse(bool isSuccess, int skillId, string message = "")
        {
            return new AttackResponse(isSuccess, skillId, message).ToBytes();
        }

        public static byte[] MoveMapResponse(bool isSuccess, int mapId, float spawnX, float spawnY, float spawnZ, string message = "")
        {
            return new MoveMapResponse(isSuccess, mapId, spawnX, spawnY, spawnZ, message).ToBytes();
        }

        public static byte[] ItemMoveResponse(bool isSuccess, int prevSlotIndex, int newSlotIndex, string message = "")
        {
            return new ItemMoveResponse(isSuccess, prevSlotIndex, newSlotIndex, message).ToBytes();
        }

        public static byte[] ItemUseResponse(bool isSuccess, int slotIndex, int remainingQuantity, string message = "")
        {
            return new ItemUseResponse(isSuccess, slotIndex, remainingQuantity, message).ToBytes();
        }

        public static byte[] ItemDropResponse(bool isSuccess, int slotIndex, int remainingQuantity, string message = "")
        {
            return new ItemDropResponse(isSuccess, slotIndex, remainingQuantity, message).ToBytes();
        }

        public static byte[] GroundItemGetResponse(bool isSuccess, int itemId, int quantity, int slotIndex, string message = "")
        {
            return new GroundItemGetResponse(isSuccess, itemId, quantity, slotIndex, message).ToBytes();
        }

        #endregion

        #region Notify

        public static byte[] ChatNotify(string nickname, string message)
        {
            return new ChatNotify(nickname, message).ToBytes();
        }

        public static byte[] PlayerMoveNotify(int playerId, Vector3 pos, float rotationY, float dirX, float dirZ, bool isMoving, float speed)
        {
            return new PlayerMoveNotify(playerId, pos.X, pos.Y, pos.Z, rotationY, dirX, dirZ, isMoving, speed).ToBytes();
        }

        public static byte[] PlayerSpawnNotify(int playerId, string nickname, PlayerPosition playerPosition)
        {
            return new PlayerSpawnNotify(playerId, nickname,
                playerPosition.Pos.X, playerPosition.Pos.Y, playerPosition.Pos.Z, playerPosition.RotationY,
                playerPosition.DirectionX, playerPosition.DirectionZ, playerPosition.IsMoving,
                Constants.MaxPlayerMoveSpeed).ToBytes();
        }

        public static byte[] PlayerLeaveNotify(int playerId)
        {
            return new PlayerLeaveNotify(playerId).ToBytes();
        }

        public static byte[] AssignPlayerIdNotify(int playerId)
        {
            return new AssignPlayerIdNotify(playerId).ToBytes();
        }

        public static byte[] EnterCompleteNotify()
        {
            long serverTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return new EnterCompleteNotify(serverTime).ToBytes();
        }

        public static byte[] PlayerStatsNotify(PlayerStats stats)
        {
            return new PlayerStatsNotify(
                stats.Level,
                stats.Exp,
                DataManager.GetNextLevelRequiredExp(stats.Level),
                stats.Hp,
                stats.MaxHp,
                stats.Mp,
                stats.MaxMp
            ).ToBytes();
        }

        public static byte[] HpChangeNotify(int hp, int maxHp)
        {
            return new HpChangeNotify(hp, maxHp).ToBytes();
        }

        public static byte[] MpChangeNotify(int mp, int maxMp)
        {
            return new MpChangeNotify(mp, maxMp).ToBytes();
        }

        public static byte[] ExpChangeNotify(int exp, int requiredExp)
        {
            return new ExpChangeNotify(exp, requiredExp).ToBytes();
        }

        public static byte[] LevelUpNotify(int playerId, int level)
        {
            return new LevelUpNotify(playerId, level).ToBytes();
        }

        public static byte[] PlayerDeathNotify(int playerId)
        {
            return new PlayerDeathNotify(playerId).ToBytes();
        }

        public static byte[] RespawnNotify(int playerId, string nickname, int mapId, Vector3 pos, float rotationY)
        {
            return new RespawnNotify(playerId, nickname, mapId, pos.X, pos.Y, pos.Z, rotationY).ToBytes();
        }

        public static byte[] AttendanceNotify(int attendanceId, int dayCount, bool isCheckedToday)
        {
            return new AttendanceNotify(attendanceId, dayCount, isCheckedToday).ToBytes();
        }

        public static byte[] AttackNotify(int playerId, int skillId, Vector3 dir, Vector3 pos)
        {
            return new AttackNotify(playerId, skillId, dir.X, dir.Y, dir.Z, pos.X, pos.Y, pos.Z).ToBytes();
        }

        public static byte[] DamageNotify(Projectile projectile, Session target)
        {
            return new DamageNotify(projectile.OwnerId, target.PlayerId, projectile.SkillData.Damage, target.Stats.Hp, target.Stats.MaxHp).ToBytes();
        }

        public static byte[] MonsterSpawnNotify(int mapId, int monsterId, int uniqueId, Vector3 pos)
        {
            return new MonsterSpawnNotify(mapId, monsterId, uniqueId, pos.X, pos.Y, pos.Z).ToBytes();
        }

        public static byte[] MonsterDeathNotify(int uniqueId)
        {
            return new MonsterDeathNotify(uniqueId).ToBytes();
        }

        public static byte[] MonsterDamageNotify(int attackerId, MonsterInstance monster, int damage)
        {
            return new MonsterDamageNotify(attackerId, monster.UniqueId, damage, monster.Hp, monster.MaxHp).ToBytes();
        }

        public static byte[] MonsterMoveNotify(int uniqueId, Vector3 startPos, Vector3 targetPos, long startTime, long arriveTime)
        {
            return new MonsterMoveNotify(uniqueId, startPos.X, startPos.Y, startPos.Z, targetPos.X, targetPos.Y, targetPos.Z, startTime, arriveTime).ToBytes();
        }

        public static byte[] GroundItemSpawnNotify(int groundItemId, int itemId, int quantity, Vector3 spawnPos)
        {
            return new GroundItemSpawnNotify(groundItemId, itemId, quantity, spawnPos.X, spawnPos.Y, spawnPos.Z).ToBytes();
        }

        public static byte[] GroundItemDespawnNotify(int groundItemId)
        {
            return new GroundItemDespawnNotify(groundItemId).ToBytes();
        }

        public static byte[] ItemAddNotify(int itemId, int quantity, int slotIndex)
        {
            return new ItemAddNotify(itemId, quantity, slotIndex).ToBytes();
        }

        #endregion
    }
}

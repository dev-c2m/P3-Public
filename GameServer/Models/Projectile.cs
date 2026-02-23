using System.Numerics;
using UnityServer.Data;

namespace UnityServer.Models
{
    public class Projectile
    {
        public int ProjectileId { get; }
        public int OwnerId { get; }
        public int MapId { get; }
        public Vector3 StartPos { get; }
        public Vector3 Direction { get; }
        public SkillData SkillData { get; }
        public long FiredAtMs { get; }

        public Projectile(int projectileId, int ownerId, int mapId, Vector3 startPos, Vector3 direction, SkillData skillData)
        {
            ProjectileId = projectileId;
            OwnerId = ownerId;
            MapId = mapId;
            StartPos = startPos;
            SkillData = skillData;
            FiredAtMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            float magnitude = direction.Length();
            Direction = magnitude > 0 ? direction / magnitude : Vector3.Zero;
        }

        public Vector3 GetCurrentPosition(long nowMs)
        {
            float elapsed = (nowMs - FiredAtMs) / 1000f;
            return StartPos + Direction * SkillData.ProjectileSpeed * elapsed;
        }

        public float GetTraveledDistance(long nowMs)
        {
            float elapsed = (nowMs - FiredAtMs) / 1000f;
            return SkillData.ProjectileSpeed * elapsed;
        }
    }
}

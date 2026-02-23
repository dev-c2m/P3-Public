using System.Numerics;
using UnityServer.Data;
using UnityServer.Models;
using UnityServer.Share;

namespace UnityServer
{
    class ProjectileManager
    {
        private Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
        private int nextProjectileId = 1;
        private readonly Random dropRandom = new Random();

        public void AddProjectile(int ownerId, int mapId, SkillData skillData, Vector3 startPos, Vector3 direction)
        {
            int projectileId = nextProjectileId++;
            Projectile projectile = new Projectile(projectileId, ownerId, mapId, startPos, direction, skillData);
            projectiles[projectileId] = projectile;
        }

        public void ProcessProjectiles()
        {
            long nowMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long prevMs = nowMs - Constants.SendTick;
            List<int> removeIds = new List<int>();

            foreach (Projectile projectile in projectiles.Values)
            {
                float traveledDistance = projectile.GetTraveledDistance(nowMs);

                if (traveledDistance > projectile.SkillData.Range)
                {
                    removeIds.Add(projectile.ProjectileId);
                    continue;
                }

                Vector3 prevPos = projectile.GetCurrentPosition(prevMs);
                Vector3 currPos = projectile.GetCurrentPosition(nowMs);

                bool hit = CheckMonsterCollision(projectile, prevPos, currPos, nowMs)
                        || CheckPlayerCollision(projectile, prevPos, currPos);

                if (hit)
                {
                    removeIds.Add(projectile.ProjectileId);
                }
            }

            foreach (int id in removeIds)
            {
                projectiles.Remove(id);
            }
        }

        private bool CheckMonsterCollision(Projectile projectile, Vector3 prevPos, Vector3 currPos, long nowMs)
        {
            List<MonsterInstance> monsters = Program.MonsterManager.GetMonstersInMap(projectile.MapId);

            foreach (MonsterInstance monster in monsters)
            {
                if (monster.IsDead)
                    continue;

                float monsterX = monster.GetCurrentX(nowMs);
                float monsterZ = monster.GetCurrentZ(nowMs);

                float dist = PointToLineDistance2D(monsterX, monsterZ, prevPos.X, prevPos.Z, currPos.X, currPos.Z);
                if (dist <= Constants.ProjectileHitRadius)
                {
                    Vector3 hitPos = new Vector3(monsterX, monster.Pos.Y, monsterZ);
                    HandleMonsterHit(projectile, monster, hitPos);
                    return true;
                }
            }
            return false;
        }

        private bool CheckPlayerCollision(Projectile projectile, Vector3 prevPos, Vector3 currPos)
        {
            IEnumerable<Session> sessions = Program.SessionManager.GetLoggedInSessionsInMap(projectile.MapId);
            foreach (Session targetSession in sessions)
            {
                if (targetSession.PlayerId == projectile.OwnerId)
                    continue;

                if (targetSession.Stats.IsDead)
                    continue;

                float dist = PointToLineDistance2D(targetSession.Position.Pos.X, targetSession.Position.Pos.Z, prevPos.X, prevPos.Z, currPos.X, currPos.Z);

                if (dist <= Constants.ProjectileHitRadius)
                {
                    HandlePlayerHit(projectile, targetSession);
                    return true;
                }
            }
            return false;
        }

        private float PointToLineDistance2D(float targetX, float targetZ, float ax, float az, float bx, float bz)
        {
            float dx = 0;
            float dz = 0;
            float lineX = bx - ax;
            float lineZ = bz - az;
            float lineLength = lineX * lineX + lineZ * lineZ;

            if (lineLength < 0.0001f)
            {
                dx = targetX - ax;
                dz = targetZ - az;
                return MathF.Sqrt(dx * dx + dz * dz);
            }

            float t = (lineX * (targetX - ax) + lineZ * (targetZ - az)) / lineLength;
            t = MathF.Max(0f, MathF.Min(1f, t));

            float lerpX = ax + t * lineX;
            float lerpZ = az + t * lineZ;

            dx = targetX - lerpX;
            dz = targetZ - lerpZ;
            return MathF.Sqrt(dx * dx + dz * dz);
        }

        private void HandleMonsterHit(Projectile projectile, MonsterInstance monster, Vector3 hitPos)
        {
            monster.TakeDamage(projectile.SkillData.Damage);

            IEnumerable<Session> sessionsInMap = Program.SessionManager.GetLoggedInSessionsInMap(projectile.MapId);

            byte[] damageNotify = PacketWriter.MonsterDamageNotify(projectile.OwnerId, monster, projectile.SkillData.Damage);

            foreach (Session s in sessionsInMap)
            {
                s.SendAsync(damageNotify);
            }

            bool isDead = CheckMonsterDeath(monster, sessionsInMap);

            if (isDead)
            {
                MonsterData? monsterData = DataManager.GetMonsterData(monster.MonsterId);
                if (monsterData == null)
                    return;

                GiveReward(projectile.OwnerId, monsterData.Exp);                     
                DropItems(hitPos, monster.MapId, monsterData, sessionsInMap);
            }
        }

        private void HandlePlayerHit(Projectile projectile, Session targetSession)
        {
            targetSession.TakeDamage(projectile.SkillData.Damage);

            byte[] damageNotify = PacketWriter.DamageNotify(projectile, targetSession);
            IEnumerable<Session> sessionsInMap = Program.SessionManager.GetLoggedInSessionsInMap(projectile.MapId);

            foreach (Session s in sessionsInMap)
            {
                s.SendAsync(damageNotify);
            }

            if (targetSession.Stats.IsDead)
            {
                byte[] deathNotify = PacketWriter.PlayerDeathNotify(targetSession.PlayerId);
                foreach (Session s in sessionsInMap)
                {
                    s.SendAsync(deathNotify);
                }
            }
        }

        private bool CheckMonsterDeath(MonsterInstance monster, IEnumerable<Session> sessionsInMap)
        {
            if (!monster.IsDead)
                return false;

            byte[] deathNotify = PacketWriter.MonsterDeathNotify(monster.UniqueId);
            foreach (Session s in sessionsInMap)
            {
                s.SendAsync(deathNotify);
            }

            Program.MonsterManager.RemoveMonster(monster.UniqueId);

            return true;
        }

        private void GiveReward(int projectileOwnerId, int exp)
        {
            Session? ownerSession = Program.SessionManager.GetSessionById(projectileOwnerId);
            if (ownerSession == null)
                return;

            ownerSession.AddExp(exp);
        }

        private void DropItems(Vector3 deathPos, int mapId, MonsterData monsterData, IEnumerable<Session> sessionsInMap)
        {
            foreach (MonsterDropData drop in monsterData.Drops)
            {
                float roll = (float)(dropRandom.NextDouble() * 100);
                if (roll >= drop.DropRate)
                    continue;

                float offsetX = (float)(dropRandom.NextDouble() * 2 - 1) * Constants.GroundItemDropOffset;
                float offsetZ = (float)(dropRandom.NextDouble() * 2 - 1) * Constants.GroundItemDropOffset;

                Vector3 dropPos = new Vector3(deathPos.X + offsetX, deathPos.Y, deathPos.Z + offsetZ);
                GroundItemInstance groundItem = Program.GroundItemManager.AddItem(drop.ItemId, drop.Quantity, mapId, dropPos);

                byte[] notify = PacketWriter.GroundItemSpawnNotify(groundItem.UniqueId, groundItem.ItemId, groundItem.Quantity, dropPos);

                foreach (Session s in sessionsInMap)
                {
                    s.SendAsync(notify);
                }
            }
        }
    }
}

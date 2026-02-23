using System.Numerics;

namespace UnityServer.Models
{
    public class MonsterInstance
    {
        public int UniqueId { get; }
        public int MonsterId { get; }
        public int MapId { get; }
        public int Hp { get; private set; }
        public int MaxHp { get; }
        public Vector3 Pos { get; private set; }
        public bool IsDead => Hp <= 0;

        // 이동 관련
        public Vector3 TargetPos { get; private set; }
        public float MoveSpeed { get; private set; }
        public DateTime NextMoveAt { get; private set; }
        public long StartTimeMs { get; private set; }
        public long ArriveTimeMs { get; private set; }

        public MonsterInstance(int uniqueId, int monsterId, int mapId, int maxHp, Vector3 pos)
        {
            UniqueId = uniqueId;
            MonsterId = monsterId;
            MapId = mapId;
            MaxHp = maxHp;
            Hp = maxHp;
            Pos = pos;
            NextMoveAt = DateTime.UtcNow.AddSeconds(new Random().Next(5, 8));
            MoveSpeed = Constants.MonsterMoveSpeed;
        }

        public void TakeDamage(int damage)
        {
            Hp = Math.Max(0, Hp - damage);
        }

        public float GetCurrentX(long nowMs)
        {
            if (ArriveTimeMs <= 0)
                return Pos.X;

            if (nowMs >= ArriveTimeMs)
                return TargetPos.X;

            if (nowMs <= StartTimeMs)
                return Pos.X;

            float t = (float)(nowMs - StartTimeMs) / (ArriveTimeMs - StartTimeMs);
            return Pos.X + ((TargetPos.X - Pos.X) * t);
        }

        public float GetCurrentZ(long nowMs)
        {
            if (ArriveTimeMs <= 0)
                return Pos.Z;

            if (nowMs >= ArriveTimeMs)
                return TargetPos.Z;

            if (nowMs <= StartTimeMs)
                return Pos.Z;

            float t = (float)(nowMs - StartTimeMs) / (ArriveTimeMs - StartTimeMs);
            return Pos.Z + ((TargetPos.Z - Pos.Z) * t);
        }

        public void Arrived()
        {
            Pos = TargetPos;
            ArriveTimeMs = 0;
        }

        public void UpdateNextMove(Vector3 nextPos, long startMs, long arriveMs, DateTime nextMoveAt)
        {
            TargetPos = nextPos;
            StartTimeMs = startMs;
            ArriveTimeMs = arriveMs;
            NextMoveAt = nextMoveAt;
        }

        public float Distance(Vector3 targetPos)
        {
            return Vector3.Distance(Pos, targetPos);
        }
    }
}

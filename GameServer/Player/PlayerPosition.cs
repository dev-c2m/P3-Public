using System.Numerics;

namespace UnityServer.Player
{
    public class PlayerPosition
    {
        public Vector3 Pos { get; private set; }
        public float RotationY { get; private set; }
        public DateTime LastMoveAt { get; private set; } = DateTime.Now;
        public int MapId { get; private set; }
        public DateTime? TeleportedAt { get; private set; }

        public float DirectionX { get; private set; }
        public float DirectionZ { get; private set; }
        public bool IsMoving { get; private set; }

        public void SetMovementInput(float dirX, float dirZ, bool isMoving, float rotationY)
        {
            DirectionX = dirX;
            DirectionZ = dirZ;
            IsMoving = isMoving;
            RotationY = rotationY;
        }

        public bool TickMovement(float deltaTime, float mapSize)
        {
            if (!IsMoving)
                return false;

            float moveDistance = Constants.MaxPlayerMoveSpeed * deltaTime;
            float newX = Pos.X + DirectionX * moveDistance;
            float newZ = Pos.Z + DirectionZ * moveDistance;

            float clampedX = MathF.Max(-mapSize, MathF.Min(mapSize, newX));
            float clampedZ = MathF.Max(-mapSize, MathF.Min(mapSize, newZ));

            bool clamped = (newX != clampedX || newZ != clampedZ);

            Pos = new Vector3(clampedX, Pos.Y, clampedZ);
            LastMoveAt = DateTime.Now;

            return clamped;
        }

        public void StopMovement()
        {
            IsMoving = false;
            DirectionX = 0;
            DirectionZ = 0;
        }


        public void Teleport(Vector3 pos, float rotationY)
        {
            Pos = pos;
            RotationY = rotationY;
            TeleportedAt = DateTime.Now;
        }

        public void SetMapId(int mapId)
        {
            MapId = mapId;
        }

        public float DistanceTo(PlayerPosition other)
        {
            return Vector3.Distance(Pos, other.Pos);
        }
    }
}

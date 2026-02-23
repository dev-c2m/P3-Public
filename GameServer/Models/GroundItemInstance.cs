using System.Numerics;

namespace UnityServer.Models
{
    public class GroundItemInstance
    {
        public int UniqueId { get; }
        public int ItemId { get; }
        public int Quantity { get; }
        public int MapId { get; }
        public Vector3 DropPos {  get; }
        public DateTime DroppedAt { get; }

        public GroundItemInstance(int uniqueId, int itemId, int quantity, int mapId, Vector3 dropPos)
        {
            UniqueId = uniqueId;
            ItemId = itemId;
            Quantity = quantity;
            MapId = mapId;
            DropPos = dropPos;
            DroppedAt = DateTime.Now;
        }
    }
}

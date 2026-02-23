using System.Numerics;
using UnityServer.Models;
using UnityServer.Share;

namespace UnityServer
{
    class GroundItemManager
    {
        private Dictionary<int, GroundItemInstance> items = new Dictionary<int, GroundItemInstance>();
        private Dictionary<int, List<GroundItemInstance>> mapItems = new Dictionary<int, List<GroundItemInstance>>();
        private int nextUniqueId = 1;

        public GroundItemInstance AddItem(int itemId, int quantity, int mapId, Vector3 dropPos)
        {
            GroundItemInstance instance = new GroundItemInstance(nextUniqueId++, itemId, quantity, mapId, dropPos);
            items[instance.UniqueId] = instance;

            if (!mapItems.TryGetValue(mapId, out List<GroundItemInstance> list))
            {
                list = new List<GroundItemInstance>();
                mapItems[mapId] = list;
            }
            list.Add(instance);

            return instance;
        }

        public GroundItemInstance? GetItem(int uniqueId)
        {
            return items.TryGetValue(uniqueId, out GroundItemInstance item) ? item : null;
        }

        public GroundItemInstance? RemoveItem(int uniqueId)
        {
            if (!items.TryGetValue(uniqueId, out GroundItemInstance item))
                return null;

            items.Remove(uniqueId);
            if (mapItems.TryGetValue(item.MapId, out List<GroundItemInstance> list))
            {
                list.Remove(item);
            }
            return item;
        }

        public List<byte[]> GetSpawnNotirysInMap(int mapId)
        {
            List<byte[]> result = new List<byte[]>();

            if (!mapItems.TryGetValue(mapId, out List<GroundItemInstance> list))
                return result;

            foreach (GroundItemInstance item in list)
            {
                result.Add(PacketWriter.GroundItemSpawnNotify(item.UniqueId, item.ItemId, item.Quantity, item.DropPos));
            }

            return result;
        }
    }
}

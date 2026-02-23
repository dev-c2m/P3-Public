using UnityServer.Models;

namespace UnityServer
{
    static class InventoryManager
    {
        public static InventoryItem? AddItem(Dictionary<int, InventoryItem> inventory, int itemId, int quantity)
        {
            foreach (InventoryItem inventoryItem in inventory.Values)
            {
                if (inventoryItem.ItemId == itemId)
                {
                    inventoryItem.UpdateQuantity(quantity);
                    return inventoryItem;
                }
            }

            int emptySlot = FindEmptySlot(inventory);
            if (emptySlot == -1)
                return null;

            InventoryItem item = new InventoryItem(itemId, emptySlot, quantity);
            inventory[emptySlot] = item;
            return item;
        }

        public static int RemoveItem(Dictionary<int, InventoryItem> inventory, int slotIndex, int quantity)
        {
            if (!inventory.TryGetValue(slotIndex, out InventoryItem? item))
                return -1;

            if (item.Quantity < quantity)
                return -1;

            item.UpdateQuantity(-quantity);

            if (item.Quantity <= 0)
            {
                inventory.Remove(slotIndex);
                return 0;
            }

            return item.Quantity;
        }

        public static bool MoveItem(Dictionary<int, InventoryItem> inventory, int fromSlot, int toSlot)
        {
            if (fromSlot == toSlot)
                return false;

            if (!inventory.TryGetValue(fromSlot, out InventoryItem? fromItem))
                return false;

            if (toSlot < 0 || toSlot >= Constants.MaxInventorySlots)
                return false;

            if (inventory.TryGetValue(toSlot, out InventoryItem? toItem))
            {
                fromItem.UpdateSlotIndex(toSlot);
                toItem.UpdateSlotIndex(fromSlot);
                inventory[toSlot] = fromItem;
                inventory[fromSlot] = toItem;
            }
            else
            {
                fromItem.UpdateSlotIndex(toSlot);
                inventory.Remove(fromSlot);
                inventory[toSlot] = fromItem;
            }

            return true;
        }

        public static InventoryItem? GetItem(Dictionary<int, InventoryItem> inventory, int slotIndex)
        {
            return inventory.TryGetValue(slotIndex, out InventoryItem? item) ? item : null;
        }

        private static int FindEmptySlot(Dictionary<int, InventoryItem> inventory)
        {
            for (int i = 0; i < Constants.MaxInventorySlots; i++)
            {
                if (!inventory.ContainsKey(i))
                    return i;
            }
            return -1;
        }
    }
}

using UnityServer.Models;

namespace UnityServer.Repository.Interface
{
    public interface IInventoryRepository
    {
        List<InventoryItem> GetItems(int accountId);
        void InsertItem(int accountId, int slotIndex, int itemId, int qty);
        void UpdateItem(int accountId, int slotIndex, int qty);
        void DeleteItem(int accountId, int slotIndex);
        void MoveItem(int accountId, int fromSlot, int toSlot);
        void SwapItems(int accountId, int slotA, int slotB);
    }
}

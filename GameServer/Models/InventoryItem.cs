namespace UnityServer.Models
{
    public class InventoryItem
    {
        public int ItemId { get; private set; }
        public int SlotIndex { get; private set; }
        public int Quantity { get; private set; }

        public InventoryItem(int itemId, int slotIndex, int quantity)
        {
            ItemId = itemId;
            SlotIndex = slotIndex;
            Quantity = quantity;
        }

        public void UpdateSlotIndex(int newSlotIndex)
        {
            SlotIndex = newSlotIndex;
        }

        public void UpdateQuantity(int newQuantity)
        {
            Quantity += newQuantity;
        }
    }
}

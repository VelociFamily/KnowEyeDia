using System.Collections.Generic;

namespace KnowEyeDia.Domain.Entities
{
    public class InventoryEntity
    {
        public List<InventorySlot> Slots { get; private set; } = new List<InventorySlot>();
        public int CraftingGridSize { get; set; } = 3; // Starts at 3x3

        public InventoryEntity(int slotCount)
        {
            for (int i = 0; i < slotCount; i++)
            {
                Slots.Add(new InventorySlot());
            }
        }
    }

    public class InventorySlot
    {
        public ItemData Item { get; set; }
        public int Quantity { get; set; }

        public bool IsEmpty => Item == null || Quantity <= 0;
    }
}

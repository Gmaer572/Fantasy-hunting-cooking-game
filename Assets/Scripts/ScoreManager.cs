using UnityEngine;

public static class ScoreManager
{
    public static int TotalScore { get; private set; }

    public static void Calculate(InventoryController inventory)
    {
        TotalScore = 0;
        if (inventory == null || inventory.inventoryPanel == null)
            return;

        foreach (Transform slotTransform in inventory.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot == null || slot.currentItem == null)
                continue;

            Item item = slot.currentItem.GetComponent<Item>();
            if (item != null)
                TotalScore += item.scoreValue;
        }
    }
}

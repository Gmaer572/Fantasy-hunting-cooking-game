// inventoryPanel — the parent UI panel where slots are spawned
// slotPrefab — the slot UI prefab to instantiate
// slotCount — how many slots to create
// itemPrefabs — optional items to pre-fill slots on start
// On Start: spawns slotCount slots under inventoryPanel, and fills each slot with the matching itemPrefab if available
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;

    void Start()
    {
        for (int i = 0; i < slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
            if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;
            }
        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attached to inventory item GameObjects to enable drag-and-drop between slots.
///
/// OnBeginDrag  — Lifts the item out of its slot by reparenting it to the root
///                Canvas so it renders above all other UI. Makes it semi-transparent
///                and disables raycasts so pointer events reach the slot beneath.
///
/// OnDrag       — Moves the item's position to follow the pointer each frame.
///
/// OnEndDrag    — Restores full opacity and raycasts, then resolves the drop:
///                  • If dropped on a different slot that already has an item  → swap items.
///                  • If dropped on a different empty slot                     → move item.
///                  • If dropped on the original slot or no slot               → return item.
/// </summary>
public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Lifts the item to the canvas root and makes it semi-transparent for dragging
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;   // Save OG parent
        transform.SetParent(transform.root); // Above other canvas
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;            // Semi-transparent during drag
    }

    // Moves the item to follow the mouse
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    // Drops the item into the target slot, swapping or moving as needed
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;  // Re-enables raycasts
        canvasGroup.alpha = 1f;             // No longer transparent

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>(); // Slot where item dropped
        if (dropSlot == null)
        {
            GameObject item = eventData.pointerEnter;
            if (item != null)
            {
                dropSlot = item.GetComponentInParent<Slot>();
            }
        }
        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot != null)
        {
            // It's a slot under drop point
            if (dropSlot.currentItem != null)
            {
                // Slot has an item - swap items
                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                originalSlot.currentItem = dropSlot.currentItem;
                originalSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                originalSlot.currentItem = null;
            }

            // Move item into drop slot
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            // No slot under drop point
            transform.SetParent(originalParent);
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Center
    }
}
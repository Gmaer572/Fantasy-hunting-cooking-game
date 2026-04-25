using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class ItemPickupUIController : MonoBehaviour
{
    public static ItemPickupUIController Instance { get; private set; }

    public GameObject popupPrefab;
    public int maxPopups = 5;
    public float popupDuration=3f;

    private readonly Queue<GameObject> activePopups = new ();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void ShowItemPickup(string itemName, Sprite itemIcon)
    {
        if (popupPrefab == null)
        {
            Debug.LogError("ItemPickupUIController: popupPrefab is not assigned in the Inspector.", this);
            return;
        }

        if (activePopups.Count >= maxPopups)
        {
            //destroy the oldest popup to make room for the new one
            Destroy(activePopups.Dequeue());
        }

        GameObject popup = Instantiate(popupPrefab, transform);
        //get item and icon from the prefab and set them to the item name and icon
        popup.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = itemName;
        Image iconImage = popup.transform.Find("ItemIcon")?.GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.sprite = itemIcon;
        }
        activePopups.Enqueue(popup);
        StartCoroutine(FadeOutAndDestroy(popup));

    }
    private IEnumerator FadeOutAndDestroy(GameObject popup)
    {
       yield return new WaitForSeconds(popupDuration);
       if (popup == null) yield break;

       CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
       for (float timePassed = 0; timePassed < 1f; timePassed += Time.deltaTime)
       {
            if (popup == null) yield break;
            canvasGroup.alpha = 1f - timePassed;
            yield return null;
       }
       Destroy(popup);
    }
}

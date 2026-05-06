using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;

    void Start()
    {
        inventoryController = FindObjectOfType<InventoryController>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        inventoryController = FindObjectOfType<InventoryController>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Item")) return;

        Item item = collision.GetComponent<Item>();
        if (item == null) return;

        if (inventoryController == null)
            inventoryController = FindObjectOfType<InventoryController>();
        if (inventoryController == null) return;

        bool itemAdded = inventoryController.AddItem(collision.gameObject);
        if (itemAdded)
        {
            item.Pickup();
            Destroy(collision.gameObject);
        }
    }
}

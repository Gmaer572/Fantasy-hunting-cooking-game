using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Sprite[] healthSprites;
    private static HealthBarController instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Canvas duplicateCanvas = GetComponentInParent<Canvas>();
            if (duplicateCanvas != null)
            {
                Destroy(duplicateCanvas.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            return;
        }

        instance = this;

        if (healthBarImage == null)
        {
            healthBarImage = GetComponent<Image>();
        }

        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            DontDestroyOnLoad(parentCanvas.gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void UpdateHealthBar(int currentHealth)
    {
        if (healthBarImage == null)
        {
            return;
        }

        if (healthSprites == null || healthSprites.Length == 0)
        {
            Debug.LogWarning("Health sprites are not assigned.");
            return;
        }

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (currentHealth >= healthSprites.Length)
        {
            currentHealth = healthSprites.Length - 1;
        }

        healthBarImage.sprite = healthSprites[currentHealth];
    }
}

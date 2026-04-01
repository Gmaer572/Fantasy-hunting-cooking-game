using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Sprite[] healthSprites;

    private void Awake()
    {
        if (healthBarImage == null)
        {
            healthBarImage = GetComponent<Image>();
        }
    }

    public void UpdateHealthBar(int currentHealth)
    {
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
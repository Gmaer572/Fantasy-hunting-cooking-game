using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;   

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Sprite[] healthSprites;
    private static HealthBarController instance;

    public static HealthBarController Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Canvas duplicateCanvas = GetComponentInParent<Canvas>();
            if (duplicateCanvas != null)
                Destroy(duplicateCanvas.gameObject);
            else
                Destroy(gameObject);
            return;
        }

        instance = this;
        EnsureImageReference();

        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
            DontDestroyOnLoad(parentCanvas.gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureImageReference();

        if (scene.name == "gameover")
        {
            SetHealthBarVisible(false);
        }
        else
        {
            SetHealthBarVisible(true);
        }
    }

    public void UpdateHealthBar(int currentHealth)
    {
        EnsureImageReference();
        if (healthBarImage == null || healthSprites == null || healthSprites.Length == 0) return;

        currentHealth = Mathf.Clamp(currentHealth, 0, healthSprites.Length - 1);
        healthBarImage.sprite = healthSprites[currentHealth];
    }

    private void EnsureImageReference()
    {
        if (healthBarImage == null)
            healthBarImage = GetComponent<Image>();

        if (healthBarImage == null)
            healthBarImage = GetComponentInChildren<Image>(true);
    }

    private void SetHealthBarVisible(bool visible)
    {
        if (healthBarImage != null)
            healthBarImage.enabled = visible;
    }
}

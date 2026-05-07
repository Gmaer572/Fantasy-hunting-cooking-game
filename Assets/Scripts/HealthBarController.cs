using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Sprite[] healthSprites;
    [SerializeField] private Vector3 healthBarScale = new Vector3(2.0f, 2.0f, 1f);
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
            if (SceneManager.GetActiveScene().name != "winandrestart") DontDestroyOnLoad(parentCanvas.gameObject);
            else
                if (SceneManager.GetActiveScene().name != "winandrestart") DontDestroyOnLoad(gameObject);
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
        ApplyHealthBarScale();

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

    private void ApplyHealthBarScale()
    {
        if (healthBarImage == null)
            return;

        RectTransform rect = healthBarImage.rectTransform;
        if (rect != null)
        {
            rect.localScale = healthBarScale;
        }
    }

    private void SetHealthBarVisible(bool visible)
    {
        if (healthBarImage != null)
            healthBarImage.enabled = visible;
    }
}

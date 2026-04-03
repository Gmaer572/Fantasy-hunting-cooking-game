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

        if (healthBarImage == null)
            healthBarImage = GetComponent<Image>();

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "gameover")
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
                canvas.gameObject.SetActive(false);
            else
                gameObject.SetActive(false);
        }
        else
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
                canvas.gameObject.SetActive(true);
            else
                gameObject.SetActive(true);
        }
    }

    public void UpdateHealthBar(int currentHealth)
    {
        if (healthBarImage == null) return;

        currentHealth = Mathf.Clamp(currentHealth, 0, healthSprites.Length - 1);
        healthBarImage.sprite = healthSprites[currentHealth];
    }
}
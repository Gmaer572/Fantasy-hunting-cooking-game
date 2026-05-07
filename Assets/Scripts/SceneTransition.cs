using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] int spawnPoint;
    [SerializeField] float fadeDuration = 2f;
    [SerializeField] int transitionFootstepCount = 5;
    [SerializeField] float transitionFootstepInterval = 0.25f;
    // [SerializeField] int spawnOffset;
    bool isTransitioning;
    static float sceneLoadProtectUntil;

    void OnEnable()
    {
        // Prevent immediate re-trigger when the player spawns inside/near a transition collider.
        sceneLoadProtectUntil = Time.time + 0.35f;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTransitioning || Time.time < sceneLoadProtectUntil || !collision.CompareTag("Player")) return;

        if (string.IsNullOrWhiteSpace(sceneToLoad))
        {
            Debug.LogError($"SceneTransition on {name} has no target scene assigned.");
            return;
        }

        SpawnPointHandler handler = SpawnPointHandler.Instance;
        if (handler == null)
        {
            Debug.LogError($"SceneTransition on {name} could not find SpawnPointHandler.");
            return;
        }

        isTransitioning = true;
        handler.setSpawnPoint(spawnPoint);
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        Image fadeImage = CreateFadeOverlay();
        FadeOverlayRunner runner = fadeImage.canvas.gameObject.AddComponent<FadeOverlayRunner>();

        StartCoroutine(PlayTransitionFootsteps());
        yield return StartCoroutine(Fade(fadeImage, 0f, 1f, fadeDuration));

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        loadOp.allowSceneActivation = false;
        while (loadOp.progress < 0.9f)
            yield return null;

        // Hand off fade-in to the persistent runner before this object is destroyed.
        runner.BeginFadeIn(fadeImage, fadeDuration * 0.5f, transitionFootstepCount, transitionFootstepInterval);
        loadOp.allowSceneActivation = true;
    }
    public void InvokeWin()
    {
        Invoke(nameof(loadWin), 2.0f);
    }
    public void loadWin()
    {
        SceneFadeLoader.LoadScene("winandrestart");
    }
    IEnumerator PlayTransitionFootsteps()
    {
        for (int i = 0; i < transitionFootstepCount; i++)
        {
            SoundEffectManager.Play("footstep");
            yield return new WaitForSeconds(transitionFootstepInterval);
        }
    }

    IEnumerator Fade(Image image, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(from, to, elapsed / duration);
            image.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
        image.color = new Color(0f, 0f, 0f, to);
    }

    Image CreateFadeOverlay()
    {
        GameObject canvasGO = new GameObject("FadeOverlay");
        Object.DontDestroyOnLoad(canvasGO);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGO.AddComponent<CanvasScaler>();

        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);

        Image image = imageGO.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0f);
        RectTransform rt = image.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        return image;
    }

    public string getSceneToLoad() => sceneToLoad;
    // public int getOffset() => spawnOffset;
}

// Runs on the DontDestroyOnLoad canvas so it survives the scene load and can fade back in.
public class FadeOverlayRunner : MonoBehaviour
{
    public void BeginFadeIn(Image image, float duration, int footstepCount, float footstepInterval)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        _image = image;
        _duration = duration;
        _footstepCount = footstepCount;
        _footstepInterval = footstepInterval;
    }

    Image _image;
    float _duration;
    int _footstepCount;
    float _footstepInterval;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        StartCoroutine(PlayFootsteps());
        float elapsed = 0f;
        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, elapsed / _duration);
            _image.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator PlayFootsteps()
    {
        for (int i = 0; i < _footstepCount; i++)
        {
            SoundEffectManager.Play("footstep");
            yield return new WaitForSeconds(_footstepInterval);
        }
    }
}

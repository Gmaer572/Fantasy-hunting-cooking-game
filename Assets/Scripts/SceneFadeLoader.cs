using System.Collections;
using System.Dynamic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFadeLoader : MonoBehaviour
{
    private static SceneFadeLoader instance;

    [SerializeField] private float fadeOutDuration = 0.45f;
    [SerializeField] private float fadeInDuration = 0.28f;
    [SerializeField] private int overlaySortOrder = 1000;

    private bool isTransitioning;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public static void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            return;
        }

        EnsureInstance();
        if (instance == null || instance.isTransitioning)
        {
            return;
        }

        instance.StartCoroutine(instance.LoadSceneRoutine(sceneName));
    }

    private static void EnsureInstance()
    {
        if (instance != null)
        {
            return;
        }

        GameObject go = new GameObject("SceneFadeLoader");
        instance = go.AddComponent<SceneFadeLoader>();
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isTransitioning = true;

        Image fadeImage = CreateFadeOverlay();
        yield return StartCoroutine(Fade(fadeImage, 0f, 1f, fadeOutDuration));

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!loadOp.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(Fade(fadeImage, 1f, 0f, fadeInDuration));

        if (fadeImage != null && fadeImage.canvas != null)
        {
            Destroy(fadeImage.canvas.gameObject);
        }

        isTransitioning = false;
    }

    private Image CreateFadeOverlay()
    {
        GameObject canvasGO = new GameObject("GlobalFadeOverlay");
        DontDestroyOnLoad(canvasGO);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = overlaySortOrder;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

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

    private IEnumerator Fade(Image image, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
            float alpha = Mathf.Lerp(from, to, t);
            if (image != null)
            {
                image.color = new Color(0f, 0f, 0f, alpha);
            }
            yield return null;
        }

        if (image != null)
        {
            image.color = new Color(0f, 0f, 0f, to);
        }
    }
}

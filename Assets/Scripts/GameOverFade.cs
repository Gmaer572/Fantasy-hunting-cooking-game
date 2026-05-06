using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverFade : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1.5f;

    private void Start()
    {
        StartCoroutine(FadeFromWhite());
    }

    private IEnumerator FadeFromWhite()
    {
        Image overlay = CreateOverlay();
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            overlay.color = new Color(1f, 1f, 1f, a);
            yield return null;
        }
        Destroy(overlay.transform.parent.gameObject);
    }

    private Image CreateOverlay()
    {
        GameObject canvasGO = new GameObject("WhiteFadeOverlay");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGO.AddComponent<CanvasScaler>();

        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);
        Image image = imageGO.AddComponent<Image>();
        image.color = Color.white;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        return image;
    }
}

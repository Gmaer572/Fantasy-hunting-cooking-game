using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private static Timer instance;

    [Header("Timer")]
    [SerializeField] private float defaultTime = 100f;
    [SerializeField] private bool useDaySystem = true;
    [SerializeField] private Key endDayHotkey = Key.F;

    [Header("UI (same pattern as HealthBar)")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Vector3 timerScale = new Vector3(1.2f, 1.2f, 1f);

    private float timeRemaining;
    private bool hasTriggeredSceneChange;

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
        EnsureTextReference();
        ApplyTimerStyle();

        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
            DontDestroyOnLoad(parentCanvas.gameObject);

        else
            DontDestroyOnLoad(gameObject);


        if (defaultTime <= 0f)
            defaultTime = 100f;

        timeRemaining = defaultTime;
        hasTriggeredSceneChange = false;

        if (useDaySystem && DayManager.Instance == null)
        {
            GameObject managerObj = new GameObject("DayManager");
            managerObj.AddComponent<DayManager>();
        }

        string sceneName = SceneManager.GetActiveScene().name;
        bool timerActive = IsTimerActiveScene(sceneName);
        if (timerText != null) timerText.enabled = timerActive;
        if (!timerActive)
        {
            timeRemaining = defaultTime;
            hasTriggeredSceneChange = false;
        }
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
        EnsureTextReference();
        ApplyTimerStyle();

        bool timerActive = IsTimerActiveScene(scene.name);
        if (!timerActive)
        {
            if (timerText != null) timerText.enabled = false;
            timeRemaining = defaultTime;
            hasTriggeredSceneChange = false;
        }
        else
        {
            if (timerText != null) timerText.enabled = true;
        }


    }

    private void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        ApplyTimerStyle();

        if (!IsTimerActiveScene(currentScene))
            return;

        if (hasTriggeredSceneChange)
            return;

        if (PauseController.IsGamePaused)
            return;

        if (Keyboard.current != null && Keyboard.current[endDayHotkey].wasPressedThisFrame)
            timeRemaining = 0f;

        if (Input.GetKeyDown(KeyCode.F))
            timeRemaining = 0f;

        timeRemaining -= Time.deltaTime;

        if (timerText != null)
        {
            int day = DayManager.Instance != null ? DayManager.Instance.CurrentDay : 1;
            timerText.text = useDaySystem
                ? $"Day {day}  Time: {Mathf.CeilToInt(timeRemaining)}"
                : $"Time: {Mathf.CeilToInt(timeRemaining)}";
        }

        if (timeRemaining <= 0f)
        {
            if (useDaySystem && DayManager.Instance != null)
            {
                hasTriggeredSceneChange = DayManager.Instance.ReturnToDialogue();
            }
            else
            {
                SceneManager.LoadScene("gameover");
            }
        }
    }

    private void EnsureTextReference()
    {
        if (timerText == null)
            timerText = GetComponent<TextMeshProUGUI>();

        if (timerText == null)
            timerText = GetComponentInChildren<TextMeshProUGUI>(true);

        if (timerText == null)
        {
            GameObject go = GameObject.Find("TimerText");
            if (go != null)
                timerText = go.GetComponent<TextMeshProUGUI>();
        }

        if (timerText == null)
        {
            CreateTimerTextIfMissing();
        }
    }

    private void ApplyTimerStyle()
    {
        if (timerText == null)
            return;

        RectTransform rect = timerText.rectTransform;
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-240f, -24f);
        rect.sizeDelta = new Vector2(540f, 64f);
        rect.localScale = timerScale;

        timerText.fontSize = 34f;
        timerText.alignment = TextAlignmentOptions.TopRight;
        timerText.color = new Color(0.95f, 0.93f, 0.78f, 1f);
        timerText.outlineWidth = 0.2f;
        timerText.outlineColor = new Color(0.12f, 0.18f, 0.12f, 1f);
    }

    private void CreateTimerTextIfMissing()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
        }

        if (canvas == null)
        {
            GameObject canvasGo = new GameObject("HUDCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        GameObject textGo = new GameObject("TimerText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGo.transform.SetParent(canvas.transform, false);
        timerText = textGo.GetComponent<TextMeshProUGUI>();
    }

    private bool IsTimerActiveScene(string sceneName)
    {
        if (sceneName == "title" || sceneName == "gameover" || sceneName == "winandrestart")
            return false;

        if (DayManager.Instance != null && DayManager.Instance.IsDialogueScene(sceneName))
            return false;

        return true;
    }
}

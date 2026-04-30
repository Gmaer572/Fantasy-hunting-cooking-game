using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class Timer : MonoBehaviour
{
    static Timer instance;
    [SerializeField] float defaultTime = 100f;
    [SerializeField] bool useDaySystem = true;
    [SerializeField] Key endDayHotkey = Key.F;

    float timeRemaining;
    TextMeshProUGUI timerText;
    bool hasTriggeredSceneChange;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        timerText = GetComponent<TextMeshProUGUI>();

        if (defaultTime <= 0f)
        {
            defaultTime = 100f;
        }

        timeRemaining = defaultTime;
        hasTriggeredSceneChange = false;
        DontDestroyOnLoad(gameObject);

        if (useDaySystem && DayManager.Instance == null)
        {
            GameObject managerObj = new GameObject("DayManager");
            managerObj.AddComponent<DayManager>();
        }
    }

    void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (useDaySystem && DayManager.Instance != null)
        {
            if (DayManager.Instance.IsDialogueScene(currentScene))
            {
                if (timerText != null) timerText.enabled = false;
                timeRemaining = defaultTime;
                hasTriggeredSceneChange = false;
                return;
            }
        }

        if (currentScene == "gameover")
        {
            if (timerText != null)
            {
                timerText.enabled = false;
            }

            timeRemaining = defaultTime;
            return;
        }

        if (timerText != null && !timerText.enabled)
        {
            timerText.enabled = true;
        }

        if (hasTriggeredSceneChange)
        {
            return;
        }

        if (useDaySystem && DayManager.Instance != null && Keyboard.current != null)
        {
            if (Keyboard.current[endDayHotkey].wasPressedThisFrame)
            {
                timeRemaining = 0f;
            }
        }

        // Fallback for cases where Input System keyboard focus is not active in Game view.
        if (Input.GetKeyDown(KeyCode.F))
        {
            timeRemaining = 0f;
        }

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
                bool changed = DayManager.Instance.ReturnToDialogue();
                hasTriggeredSceneChange = changed;
            }
            else
            {
                SceneManager.LoadScene("gameover");
            }
        }
    }
}

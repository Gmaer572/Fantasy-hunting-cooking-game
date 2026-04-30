using UnityEngine;
using UnityEngine.SceneManagement;

public class DayManager : MonoBehaviour
{
    private static DayManager instance;
    public static DayManager Instance => instance;

    [Header("Day Flow")]
    [SerializeField] private int startDay = 0;
    [SerializeField] private int maxDay = 30;
    [SerializeField] private string dialogueSceneName = "Dialogue";
    [SerializeField] private string gameplaySceneName = "Room1";
    [SerializeField] private string gameOverSceneName = "gameover";
    [SerializeField] private bool useMaxDayLimit = false;

    public int CurrentDay { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (startDay < 0) startDay = 0;
        if (maxDay < 0) maxDay = 0;

        CurrentDay = startDay;
    }

    public bool IsDialogueScene(string sceneName)
    {
        return sceneName == dialogueSceneName;
    }

    public bool IsGameOverScene(string sceneName)
    {
        return sceneName == gameOverSceneName;
    }

    public bool IsGameplayScene(string sceneName)
    {
        return sceneName == gameplaySceneName;
    }

    public bool ReturnToDialogue()
    {
        if (string.IsNullOrWhiteSpace(dialogueSceneName) || !Application.CanStreamedLevelBeLoaded(dialogueSceneName))
        {
            Debug.LogError($"Dialogue scene '{dialogueSceneName}' cannot be loaded. Add it to Build Settings.");
            return false;
        }

        SceneManager.LoadScene(dialogueSceneName);
        return true;
    }

    public void OnDialogueFinished()
    {
        CurrentDay++;

        if (useMaxDayLimit && CurrentDay > maxDay)
        {
            SceneManager.LoadScene(gameOverSceneName);
            return;
        }

        SceneManager.LoadScene(gameplaySceneName);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Timer : MonoBehaviour
{
    static Timer instance;
    [SerializeField] float defaultTime = 100f;

    float timeRemaining;
    TextMeshProUGUI timerText;

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
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "gameover")
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

        timeRemaining -= Time.deltaTime;
        if (timerText != null)
        {
            timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}";
        }

        if (timeRemaining <= 0f)
        {
            SceneManager.LoadScene("gameover");
        }
    }
}

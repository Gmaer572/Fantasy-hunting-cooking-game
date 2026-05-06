using UnityEngine;

public class Restartbutton : MonoBehaviour
{
    [SerializeField] private string startSceneName = "Dialogue";

    public void StartGame()
    {
        SceneFadeLoader.LoadScene(startSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

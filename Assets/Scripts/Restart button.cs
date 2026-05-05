using UnityEngine;
using UnityEngine.SceneManagement;

public class Restartbutton : MonoBehaviour
{
    [SerializeField] private string startSceneName = "Room1";

    public void StartGame()
    {
        SceneManager.LoadScene(startSceneName);
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

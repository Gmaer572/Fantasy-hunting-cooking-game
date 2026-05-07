using UnityEngine;

public class Restartbutton : MonoBehaviour
{
    [SerializeField] private string startSceneName = "Dialogue";

    public void StartGame()
    {
        InventoryController inventoryController = Object.FindAnyObjectByType<InventoryController>();
        inventoryController.DeleteAllItems();
        Invoke(nameof(StartScene), 1f);
        SpawnPointHandler.Instance?.setSpawnPoint(1);
    }
    private void StartScene()
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

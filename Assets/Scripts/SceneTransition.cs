using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] int spawnPoint;
    // [SerializeField] int spawnOffset; 
    bool isTransitioning;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTransitioning || !collision.CompareTag("Player")) return;

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

        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }

    public string getSceneToLoad() => sceneToLoad;
    // public int getOffset() => spawnOffset; 
}

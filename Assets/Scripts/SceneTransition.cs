using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] int spawnPoint;
    // [SerializeField] int spawnOffset; 

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        GameObject handlerObj = GameObject.Find("SpawnPointHandler");

        SpawnPointHandler handler = handlerObj.GetComponent<SpawnPointHandler>();

        //Set the spawn point for the next scene
        handler.setSpawnPoint(spawnPoint);

        //load the new scene
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }

    public string getSceneToLoad() => sceneToLoad;
    // public int getOffset() => spawnOffset; 
}
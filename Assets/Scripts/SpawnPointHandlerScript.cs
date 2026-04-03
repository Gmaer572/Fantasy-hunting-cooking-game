using UnityEngine;

public class SpawnPointHandler : MonoBehaviour
{
    private static SpawnPointHandler instance;
    [SerializeField] private int defaultSpawnPoint =0;
    private int spawnPoint = 1;

    void Awake()
    {
        //if (instance != null && instance != this)
       // {
        //    Destroy(gameObject);
        //    return;
       // }
        instance = this;
        // Ensure we start with a valid spawn point so no lookup for SpawnPoint0 happens on the first scene load.
        spawnPoint = defaultSpawnPoint;
        DontDestroyOnLoad(gameObject);
    }

    public void setSpawnPoint(int spawnPoint)
    {
        this.spawnPoint = spawnPoint;
        Debug.Log("Spawn point set to: " + spawnPoint);
    }

    public int getSpawnPoint()
    {
        return spawnPoint;
    }
}

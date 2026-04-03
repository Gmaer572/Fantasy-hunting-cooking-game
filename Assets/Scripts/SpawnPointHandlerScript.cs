using UnityEngine;

public class SpawnPointHandler : MonoBehaviour
{
    private static SpawnPointHandler instance;
    private int spawnPoint = 0;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
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
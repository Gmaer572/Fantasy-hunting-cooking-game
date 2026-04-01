using Unity.VisualScripting;
using UnityEngine;

public class SpawnPointHandler : MonoBehaviour
{

    int spawnPoint = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DontDestroyOnLoad(this);
    }

    public void setSpawnPoint(int spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }

    public int getSpawnPoint()
    {
        return spawnPoint;
    }
}

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{

    [SerializeField]
    string sceneToLoad;

    [SerializeField]
    int spawnPoint;

    [SerializeField]
    int spawnOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("CollisionFound");
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene("IntoForest", LoadSceneMode.Single);
            GameObject.Find("SpawnPointHandler").GetComponent<SpawnPointHandler>().setSpawnPoint(spawnPoint);
        }

    }
    public string getSceneToLoad()
    {
        return sceneToLoad;
    }

    public Vector2 getPosition()
    {
        return transform.position;
    }

    public int getOffset()
    {
        return spawnOffset;
    }

}

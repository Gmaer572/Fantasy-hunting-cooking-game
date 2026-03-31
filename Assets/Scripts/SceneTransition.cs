using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{

    [SerializeField]
    string sceneToLoad;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        Debug.Log("CollisionFound");
        if (collision.collider.CompareTag("Player"))
        {
            SceneManager.LoadScene("IntoForest", LoadSceneMode.Single);
        }
    }
    public string getSceneToLoad()
    {
        return sceneToLoad;
    }
}

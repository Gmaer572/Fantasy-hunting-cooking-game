#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;

// Editor-only. When you press Play from any scene other than Room1, this
// loads Room1 first (so all DontDestroyOnLoad singletons initialize),
// then jumps straight to the scene you actually opened.
public static class DevBootstrap
{
    static string targetScene;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        targetScene = SceneManager.GetActiveScene().name;
        if (targetScene == "Room1" || targetScene == "title" || targetScene == "Dialogue") return;

        SceneManager.sceneLoaded += OnRoom1Loaded;
        SceneManager.LoadScene("Room1");
    }

    static void OnRoom1Loaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Room1") return;
        SceneManager.sceneLoaded -= OnRoom1Loaded;
        SpawnPointHandler.Instance?.setSpawnPoint(0);
        SceneManager.LoadScene(targetScene);
    }
}
#endif

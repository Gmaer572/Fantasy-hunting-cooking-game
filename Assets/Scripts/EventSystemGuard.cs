using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EventSystemGuard : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnforceSingleEventSystemAtStartup()
    {
        KeepOnlyOneEventSystem();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        KeepOnlyOneEventSystem();
    }

    private static void KeepOnlyOneEventSystem()
    {
        EventSystem[] systems = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        if (systems == null || systems.Length <= 1)
        {
            return;
        }

        EventSystem keep = systems[0];
        for (int i = 1; i < systems.Length; i++)
        {
            if (systems[i] != null)
            {
                Object.Destroy(systems[i].gameObject);
            }
        }

        if (EventSystem.current == null && keep != null)
        {
            keep.gameObject.SetActive(true);
        }
    }
}

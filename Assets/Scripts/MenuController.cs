using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;

    void Awake()
    {
        // Fallback to a child canvas if none is wired in the Inspector to avoid null refs.
        if (menuCanvas == null)
        {
            menuCanvas = GetComponentInChildren<Canvas>(true)?.gameObject;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        menuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // if (SceneManager.GetActiveScene().name == "gameover" || SceneManager.GetActiveScene().name == "winandrestart")
        //     menuCanvas.SetActive(false);
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
            {
                return;
            }
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            PauseController.SetPause(menuCanvas.activeSelf);
            SoundEffectManager.Play("uiClick");
        }
    }
}

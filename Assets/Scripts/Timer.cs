using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    private float time;
    Text timerText;
    Boolean setTime;


    float defaultTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setTime = true;
        defaultTime = 10 / Time.deltaTime;
        time = defaultTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "gameover")
        {

            gameObject.SetActive(false);
            time = defaultTime;
        }

        DontDestroyOnLoad(gameObject);
        time--;
        gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Time:" + time;
        if (time <= 0)
        {
            SceneManager.LoadScene("gameover");

        }
    }


}

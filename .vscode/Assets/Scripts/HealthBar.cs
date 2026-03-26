using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    PlayerController controller;
    TMP_Text text;

    void Start()
    {
        PlayerController[] controllers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (PlayerController candidate in controllers)
        {
            if (candidate.isActiveAndEnabled)
            {
                controller = candidate;
                break;
            }
        }

        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (controller != null)
        {
            text.text = "HP: " + controller.getPlayerHealth();
        }
    }
}

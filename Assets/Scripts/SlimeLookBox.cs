using UnityEngine;

public class SlimeLookBox : MonoBehaviour
{
    SlimeBehaviour slimeBehaviour;

    void Start()
    {
        slimeBehaviour = GetComponentInParent<SlimeBehaviour>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        UpdatePlayerDetection(collision, true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        UpdatePlayerDetection(collision, false);
    }

    void UpdatePlayerDetection(Collider2D collision, bool detected)
    {
        if (slimeBehaviour == null)
        {
            return;
        }

        PlayerController playerController = collision.GetComponent<PlayerController>();
        if (playerController == null)
        {
            playerController = collision.GetComponentInParent<PlayerController>();
        }

        if (playerController == null)
        {
            return;
        }

        slimeBehaviour.SetPlayerDetected(detected, playerController.transform);
    }
}

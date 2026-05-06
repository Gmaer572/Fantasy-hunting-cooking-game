using UnityEngine;

public class InteractablePrompt : MonoBehaviour
{
    [SerializeField] private GameObject promptIcon;

    private void Start()
    {
        if (promptIcon != null)
            promptIcon.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && promptIcon != null)
            promptIcon.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && promptIcon != null)
            promptIcon.SetActive(false);
    }
}

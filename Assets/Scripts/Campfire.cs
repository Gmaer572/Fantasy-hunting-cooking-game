using UnityEngine;

public class Campfire : MonoBehaviour
{
    [SerializeField] private float healCooldown = 2f;

    private PlayerController playerInRange;
    private float nextHealTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = other.GetComponentInParent<PlayerController>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = null;
    }

    private void Update()
    {
        if (playerInRange == null) return;
        if (!Input.GetKeyDown(KeyCode.Return)) return;
        if (Time.time < nextHealTime) return;

        playerInRange.HealFull();
        //write debug message
        Debug.Log("Player healed to full health");
        nextHealTime = Time.time + healCooldown;
    }
}

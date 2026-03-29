using UnityEngine;

public class SlimeAttackHitbox : MonoBehaviour
{
    [SerializeField]
    int damage = 1;

    SlimeBehaviour slimeBehaviour;

    void Start()
    {
        slimeBehaviour = GetComponentInParent<SlimeBehaviour>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        TryDamagePlayer(collision);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        TryDamagePlayer(collision);
    }

    void TryDamagePlayer(Collider2D collision)
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

        slimeBehaviour.TryDamagePlayer(playerController, damage);
    }
}

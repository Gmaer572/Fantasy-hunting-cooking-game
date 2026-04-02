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

        if (collision.CompareTag("Attack"))
        {
            return;
        }

        AttackController attackController = collision.GetComponent<AttackController>();
        if (attackController == null)
        {
            attackController = collision.GetComponentInParent<AttackController>();
        }
        if (attackController != null)
        {
            return;
        }

        HurtBox hurtBox = collision.GetComponent<HurtBox>();
        if (hurtBox == null)
        {
            hurtBox = collision.GetComponentInParent<HurtBox>();
        }
        if (hurtBox == null)
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

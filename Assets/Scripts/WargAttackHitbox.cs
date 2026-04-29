using UnityEngine;

public class WargAttackHitbox : MonoBehaviour
{
    [SerializeField]
    int damage = 1;

    WargBehavior wargBehavior;

    void Start()
    {
        wargBehavior = GetComponentInParent<WargBehavior>();
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
        if (wargBehavior == null)
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

        wargBehavior.TryDamagePlayer(playerController, damage);
    }
}

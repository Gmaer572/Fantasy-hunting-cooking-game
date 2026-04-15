using UnityEngine;

public class DeerHurtBox : MonoBehaviour
{
    DeerBehavior deerBehavior;

    void Start()
    {
        deerBehavior = GetComponentInParent<DeerBehavior>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (deerBehavior == null)
        {
            return;
        }

        AttackController attackController = collision.GetComponent<AttackController>();
        if (attackController == null)
        {
            attackController = collision.GetComponentInParent<AttackController>();
        }
        if (attackController == null)
        {
            return;
        }

        deerBehavior.TakeDamage(attackController.GetDamage());
    }
}

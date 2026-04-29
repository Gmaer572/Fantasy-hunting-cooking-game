using UnityEngine;

public class WargHurtBox : MonoBehaviour
{
    WargBehavior wargBehavior;

    void Start()
    {
        wargBehavior = GetComponentInParent<WargBehavior>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (wargBehavior == null)
        {
            return;
        }

        AttackController attackController = collision.GetComponent<AttackController>();
        if (attackController != null)
        {
            wargBehavior.TakeDamage(attackController.GetDamage());
        }
    }
}

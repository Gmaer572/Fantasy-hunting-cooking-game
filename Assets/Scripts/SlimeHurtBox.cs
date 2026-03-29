using UnityEngine;

public class SlimeHurtBox : MonoBehaviour
{
    SlimeBehaviour slimeBehaviour;

    void Start()
    {
        slimeBehaviour = GetComponentInParent<SlimeBehaviour>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (slimeBehaviour == null)
        {
            return;
        }

        AttackController attackController = collision.GetComponent<AttackController>();
        if (attackController != null)
        {
            slimeBehaviour.TakeDamage(attackController.GetDamage());
        }
    }
}

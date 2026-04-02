using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("No Animator found on " + gameObject.name + " or its children.", gameObject);
        }
        else
        {
            Debug.Log("Animator found: " + animator.name, gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("J pressed", gameObject);
            Attack();
        }
    }

    void Attack()
    {
        if (animator == null) return;
        animator.SetTrigger("Attack");
    }
}
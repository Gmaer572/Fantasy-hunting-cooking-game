
using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class AttackController : MonoBehaviour
{
    [SerializeField]
    int damage = 1;

    InputAction attackAction;
    BoxCollider2D boxCollider;

    Rigidbody2D parentBody;
    PlayerController controller;


    float attackDuration;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
        parentBody = GetComponentInParent<Rigidbody2D>();
        controller = GameObject.Find("Player").GetComponent<PlayerController>();
        attackDuration = controller.attackDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (attackAction != null && attackAction.WasPressedThisFrame())
        {
            boxCollider.enabled = true;
            CancelInvoke(nameof(disableCollider));
            Invoke(nameof(disableCollider), attackDuration);
        }

        if (parentBody.linearVelocityX < 0)
        {
            transform.localPosition = new Vector3(-0.024f, 0.5250067f, 0f); //CHANGE THESE VALUES WHEN SPRITES ARE CHANGED!!!
        }
        if (parentBody.linearVelocityX > 0)
        {
            transform.localPosition = new Vector3(0.1738255f, 0.5250067f, 0f);//CHANGE THESE VALUES WHEN SPRITES ARE CHANGED!!!

        }
    }

    void disableCollider()
    {
        boxCollider.enabled = false;
    }

    public int GetDamage()
    {
        return damage;
    }
}

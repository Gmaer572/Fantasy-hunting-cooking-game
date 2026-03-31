
using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class AttackController : MonoBehaviour
{
    [SerializeField]
    int damage = 1;
    [SerializeField]
    Vector3 attackOffsetRight = new Vector3(0.22f, 0.33f, 0f);
    [SerializeField]
    Vector3 attackOffsetLeft = new Vector3(-0.22f, 0.33f, 0f);

    InputAction attackAction;
    BoxCollider2D boxCollider;

    Rigidbody2D parentBody;
    PlayerController controller;
    SpriteRenderer parentSpriteRenderer;


    float attackDuration;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
        parentBody = GetComponentInParent<Rigidbody2D>();
        controller = GetComponentInParent<PlayerController>();
        parentSpriteRenderer = GetComponentInParent<SpriteRenderer>();
        attackDuration = controller.attackDuration;
        UpdateAttackPosition();
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

        UpdateAttackPosition();
    }

    void UpdateAttackPosition()
    {
        if (parentSpriteRenderer == null)
        {
            return;
        }

        transform.localPosition = parentSpriteRenderer.flipX ? attackOffsetLeft : attackOffsetRight;
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

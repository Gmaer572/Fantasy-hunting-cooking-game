
using UnityEngine;
using UnityEngine.InputSystem;
public class AttackController : MonoBehaviour
{
    [SerializeField]
    int damage = 1;
    [SerializeField]
    Vector3 attackOffsetRight = new Vector3(0.3f, 0.25f, 0f);
    [SerializeField]
    Vector3 attackOffsetLeft = new Vector3(-0.3f, 0.25f, 0f);
    [SerializeField]
    Vector2 attackHitboxSize = new Vector2(0.45f, 0.4f);

    InputAction attackAction;
    BoxCollider2D boxCollider;

    PlayerController controller;
    SpriteRenderer parentSpriteRenderer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
        boxCollider.isTrigger = true;
        boxCollider.size = attackHitboxSize;
        controller = GetComponentInParent<PlayerController>();
        parentSpriteRenderer = GetComponentInParent<SpriteRenderer>();
        UpdateAttackPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackAction != null && attackAction.WasPressedThisFrame())
        {
            boxCollider.enabled = true;
            CancelInvoke(nameof(disableCollider));
            //float attackDuration = controller != null ? controller.attackDuration : 0.2f;
            //Invoke(nameof(disableCollider), attackDuration);
        }

        UpdateAttackPosition();
    }

    void UpdateAttackPosition()
    {
        if (parentSpriteRenderer == null)
        {
            return;
        }

        transform.localPosition = parentSpriteRenderer.flipX ? attackOffsetRight : attackOffsetLeft;
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

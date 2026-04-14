using UnityEngine;

public class DeerHitBox : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float forwardOffsetMultiplier = 0.35f;

    Vector2 attackOffsetLeft;
    Vector2 attackOffsetRight;

    SpriteRenderer parentRenderer;
    DeerBehavior deerBehavior;
    BoxCollider2D hitboxCollider;
    BoxCollider2D parentBodyCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parentRenderer = GetComponentInParent<SpriteRenderer>();
        deerBehavior = GetComponentInParent<DeerBehavior>();
        hitboxCollider = GetComponent<BoxCollider2D>();
        parentBodyCollider = GetComponentInParent<BoxCollider2D>();

        BuildOffsetsFromBodyCollider();
        transform.localPosition = attackOffsetRight;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = parentRenderer.flipX ? attackOffsetLeft : attackOffsetRight;
    }

    void BuildOffsetsFromBodyCollider()
    {
        // Default fallback if parent collider is missing.
        Vector2 bodySize = new Vector2(0.54f, 0.62f);
        float bodyCenterY = 0.32f;

        if (parentBodyCollider != null)
        {
            bodySize = parentBodyCollider.size;
            bodyCenterY = parentBodyCollider.offset.y;
        }

        float forwardOffset = bodySize.x * forwardOffsetMultiplier;
        attackOffsetLeft = new Vector2(-forwardOffset, bodyCenterY);
        attackOffsetRight = new Vector2(forwardOffset, bodyCenterY);

        if (hitboxCollider != null)
        {
            hitboxCollider.offset = Vector2.zero;
            hitboxCollider.size = bodySize;
            hitboxCollider.isTrigger = true;
        }
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
        if (deerBehavior == null)
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

        deerBehavior.TryDamagePlayer(playerController, damage);
    }

}

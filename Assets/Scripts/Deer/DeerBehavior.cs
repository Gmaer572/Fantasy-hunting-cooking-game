using UnityEngine;

public class DeerBehavior : MonoBehaviour
{

    Rigidbody2D rigidBody;
    BoxCollider2D bodyCollider;
    SpriteRenderer spriteRenderer;

    bool turnAround;
    int speed;
    int tempSpeed;
    bool isDead;

    [Header("Combat")]
    [SerializeField] int health = 3;
    [SerializeField] float attackCooldown = 0.5f;
    [SerializeField] float hitCooldown = 0.1f;
    [SerializeField] float deadDisableDelay = 0.8f;

    float nextAttackTime;
    float nextHitTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        speed = 1;
        tempSpeed = speed;
        turnAround = false;
        isDead = false;

        EnsureHurtBox();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            if (rigidBody != null)
            {
                rigidBody.linearVelocity = Vector2.zero;
            }
            return;
        }

        rigidBody.linearVelocityX = speed;
        turnCheck();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Time.time < nextHitTime ? Color.red : Color.white;
        }

        if (speed < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (speed > 0)
        {
            spriteRenderer.flipX = false;
        }

    }

    void turnCheck()
    {
        if (turnAround == true)
        {
            rigidBody.linearVelocityX = 0;
            turnAround = false;
            tempSpeed = speed;
            speed = 0;
            Invoke(nameof(turn), 1.0f);

        }
    }

    void turn()
    {
        speed = -tempSpeed;

    }
    public void setTurn(bool turn)
    {
        turnAround = turn;
    }

    public bool getTurn()
    {
        return turnAround;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || Time.time < nextHitTime)
        {
            return;
        }

        nextHitTime = Time.time + hitCooldown;
        health = Mathf.Max(0, health - damage);

        if (health == 0)
        {
            isDead = true;
            speed = 0;
            if (rigidBody != null)
            {
                rigidBody.linearVelocity = Vector2.zero;
                rigidBody.simulated = false;
            }

            Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }

            Invoke(nameof(DisableSelf), deadDisableDelay);
        }
    }

    public void TryDamagePlayer(PlayerController playerController, int damage)
    {
        if (isDead || Time.time < nextAttackTime || playerController == null)
        {
            return;
        }

        nextAttackTime = Time.time + attackCooldown;
        playerController.TakeDamage(damage);
    }

    void DisableSelf()
    {
        gameObject.SetActive(false);
    }

    void EnsureHurtBox()
    {
        DeerHurtBox existing = GetComponentInChildren<DeerHurtBox>(true);
        if (existing != null)
        {
            return;
        }

        GameObject hurtboxObj = new GameObject("deer_hurtbox");
        hurtboxObj.transform.SetParent(transform, false);
        hurtboxObj.transform.localPosition = Vector3.zero;
        hurtboxObj.transform.localRotation = Quaternion.identity;
        hurtboxObj.transform.localScale = Vector3.one;

        BoxCollider2D hurtbox = hurtboxObj.AddComponent<BoxCollider2D>();
        hurtbox.isTrigger = true;

        if (bodyCollider != null)
        {
            hurtbox.offset = bodyCollider.offset;
            hurtbox.size = bodyCollider.size;
        }

        hurtboxObj.AddComponent<DeerHurtBox>();
    }
}

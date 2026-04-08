using System.Collections;
using UnityEngine;

public class SlimeBehaviour : MonoBehaviour
{
    Rigidbody2D rigidBody;
    Collider2D bodyCollider;
    SpriteRenderer spriteRenderer;
    Animator animator;

    bool jumping;
    bool isGrounded;
    bool isDead;

    [SerializeField]
    int health = 5;
    [SerializeField]
    float attackCooldown = 0.5f;
    [SerializeField]
    float hitCooldown = 0.1f;
    [SerializeField]
    float deadDisableDelay = 1.0f;
    [SerializeField]
    LayerMask groundLayers = ~0;
    [SerializeField]
    string groundTag = "Ground";
    [SerializeField]
    string alternateGroundTag = "ground";
    [SerializeField]
    float groundedCheckDistance = 0.08f;

    float nextAttackTime;
    float nextHitTime;
    Coroutine deadRoutine;
    Transform playerTransform;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        rigidBody = GetComponent<Rigidbody2D>();
        if (rigidBody == null)
        {
            rigidBody = GetComponentInParent<Rigidbody2D>();
        }

        bodyCollider = GetComponent<Collider2D>();
        if (bodyCollider == null)
        {
            bodyCollider = GetComponentInChildren<Collider2D>();
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        RefreshPlayerReference();
        jumping = false;
        isGrounded = CheckGrounded();
        ApplyAnimatorState();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        if (playerTransform == null)
        {
            RefreshPlayerReference();
        }

        if (!jumping && isGrounded)
        {
            Invoke(nameof(SlimeJump), UnityEngine.Random.Range(0.5f, 1.0f));
            jumping = true;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Time.time < nextHitTime ? Color.red : Color.white;
        }

        ApplyAnimatorState();
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        bool groundedNow = CheckGrounded();
        if (groundedNow && !isGrounded)
        {
            CancelInvoke(nameof(ResetJump));
            Invoke(nameof(ResetJump), 0.1f);
        }

        isGrounded = groundedNow;
        ApplyAnimatorState();
    }

    void SlimeJump()
    {
        if (rigidBody == null || isDead || !isGrounded)
        {
            return;
        }

        jumping = true;
        isGrounded = false;
        ApplyAnimatorState();

        float xSpeed = GetHorizontalJumpSpeedTowardPlayer();

        rigidBody.AddForce(new Vector2(xSpeed, UnityEngine.Random.Range(3.0f, 5.0f)), ForceMode2D.Impulse);
    }

    float GetHorizontalJumpSpeedTowardPlayer()
    {
        float minSpeed = 1.0f;
        float maxSpeed = 2.0f;

        if (playerTransform == null)
        {
            return UnityEngine.Random.Range(-maxSpeed, maxSpeed);
        }

        float deltaX = playerTransform.position.x - transform.position.x;
        if (Mathf.Abs(deltaX) < 0.05f)
        {
            return UnityEngine.Random.Range(-0.25f, 0.25f);
        }

        float direction = Mathf.Sign(deltaX);
        float speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
        return direction * speed;
    }

    void ResetJump()
    {
        jumping = false;
    }

    public void TakeDamage(int damage)
    {
        if (Time.time < nextHitTime || isDead)
        {
            return;
        }

        nextHitTime = Time.time + hitCooldown;
        health = Mathf.Max(0, health - damage);

        if (health == 0)
        {
            isDead = true;
            jumping = true;
            ApplyAnimatorState();

            if (animator != null)
            {
                animator.Play("slime_dead", 0, 0f);
            }

            if (deadRoutine == null)
            {
                deadRoutine = StartCoroutine(PlayDeathAndDisable());
            }
        }
    }

    public void TryDamagePlayer(PlayerController playerController, int damage)
    {
        if (Time.time < nextAttackTime || isDead)
        {
            return;
        }

        if (playerController == null)
        {
            return;
        }

        nextAttackTime = Time.time + attackCooldown;
        playerController.TakeDamage(damage);
    }

    void RefreshPlayerReference()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    bool CheckGrounded()
    {
        if (IsGroundedByContacts())
        {
            return true;
        }

        if (bodyCollider == null)
        {
            return false;
        }

        Bounds bounds = bodyCollider.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y + 0.02f);
        Vector2 size = new Vector2(bounds.size.x * 0.85f, 0.05f);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, size, 0f, Vector2.down, groundedCheckDistance);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hitCollider = hits[i].collider;
            if (IsValidGroundCollider(hitCollider))
            {
                return true;
            }
        }

        return false;
    }

    bool IsGroundedByContacts()
    {
        if (rigidBody == null)
        {
            return false;
        }

        ContactPoint2D[] contacts = new ContactPoint2D[16];
        int count = rigidBody.GetContacts(contacts);
        for (int i = 0; i < count; i++)
        {
            ContactPoint2D contact = contacts[i];
            if (contact.normal.y > 0.2f && IsValidGroundCollider(contact.collider))
            {
                return true;
            }
        }

        return false;
    }

    bool IsValidGroundCollider(Collider2D collider)
    {
        if (collider == null || collider.isTrigger)
        {
            return false;
        }

        if (rigidBody != null && collider.attachedRigidbody == rigidBody)
        {
            return false;
        }

        int otherLayerMask = 1 << collider.gameObject.layer;
        bool inGroundLayer = (groundLayers.value & otherLayerMask) != 0;
        bool matchesGroundTag = collider.CompareTag(groundTag) || collider.CompareTag(alternateGroundTag);

        if (inGroundLayer || matchesGroundTag)
        {
            return true;
        }

        return true;
    }

    void ApplyAnimatorState()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsDead", isDead);
    }

    IEnumerator PlayDeathAndDisable()
    {
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

        yield return new WaitForSeconds(deadDisableDelay);
        gameObject.SetActive(false);
    }
}

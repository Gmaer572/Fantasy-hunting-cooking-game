using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class WargBehavior : MonoBehaviour
{
    Rigidbody2D rigidBody;
    Collider2D bodyCollider;
    SpriteRenderer spriteRenderer;
    Animator animator;

    bool jumping;
    bool jumpRecover;
    bool isGrounded;

    bool attacking;

    bool charging;
    bool chargeRecover;

    bool isDead;

    string lastAttack;

    [SerializeField]
    int health = 2;
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

    [SerializeField] GameObject meatPrefab;
    bool spawnMeat;
    bool startCheckingGrounded;
    float nextAttackTime;
    float nextHitTime;
    Coroutine deadRoutine;
    Transform playerTransform;



    void Start()
    {
        spawnMeat = true;
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
        attacking = false;

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

        if (charging && rigidBody.linearVelocityX == 0)
        {
            charging = false;
        }
        if (attacking == false)
        {
            FacePlayer();
            attacking = true;
            PickNewAttack();
        }


        if (spriteRenderer != null)
        {
            spriteRenderer.color = Time.time < nextHitTime ? Color.red : Color.white;
        }

        ApplyAnimatorState();
    }

    private void PickNewAttack()
    {
        attacking = true;
        int jumpOrCharge = UnityEngine.Random.Range(0, 2);
        if (jumpOrCharge == 0)
        {
            Invoke(nameof(InvokeJump), 2);
        }
        else if (jumpOrCharge == 1)
        {
            Invoke(nameof(InvokeCharge), 2);
        }
    }

    private void InvokeCharge()
    {
        chargeRecover = true;
        Invoke(nameof(WargCharge), 3);
    }

    private void InvokeJump()
    {
        jumpRecover = true;
        Invoke(nameof(WargJump), 3);
    }
    private void WargCharge()
    {
        spriteRenderer.color = Color.white;

        attacking = true;
        charging = true;
        ApplyAnimatorState();

        float xSpeed = GetHorizontalJumpSpeedTowardPlayer();

        rigidBody.linearVelocityX = xSpeed;
        Invoke(nameof(resetCharge), 2);
        lastAttack = "Charge";

    }

    private void resetCharge()
    {
        rigidBody.linearVelocityX = 0;
        charging = false;
        attacking = false;
        Invoke(nameof(EndChargeRecover), 0.5f);
    }

    private void EndChargeRecover()
    {
        chargeRecover = false;
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        ApplyAnimatorState();
        bool groundedNow = CheckGrounded();
        if (startCheckingGrounded)
        {
            if (groundedNow && !isGrounded && jumping)
            {
                jumping = false;
                Invoke(nameof(ResetJump), 2);
            }
        }
    }

    void WargJump()
    {
        spriteRenderer.color = Color.white;
        if (rigidBody == null || isDead)
        {
            return;
        }
        jumping = true;
        attacking = true;
        isGrounded = false;
        startCheckingGrounded = false;
        Invoke(nameof(StartCheckingGrounded), 1);
        ApplyAnimatorState();

        float xSpeed = GetHorizontalJumpSpeedTowardPlayer();
        lastAttack = "Jump";
        rigidBody.AddForce(new Vector2(xSpeed, 6), ForceMode2D.Impulse);
    }

    void StartCheckingGrounded()
    {
        startCheckingGrounded = true;
    }

    float GetHorizontalJumpSpeedTowardPlayer()
    {
        if (playerTransform == null)
        {
            return 5;
        }

        float deltaX = playerTransform.position.x - transform.position.x;
        if (Mathf.Abs(deltaX) < 0.05f)
        {
            return UnityEngine.Random.Range(-0.25f, 0.25f);
        }

        float direction = Mathf.Sign(deltaX);
        if (direction < 0)
        {
            spriteRenderer.flipX = true;

        }
        else if (direction > 0)
        {
            spriteRenderer.flipX = false;
        }
        float speed = 5;
        return direction * speed;
    }

    void FacePlayer()
    {
        float deltaX = playerTransform.position.x - transform.position.x;

        float direction = Mathf.Sign(deltaX);
        if (direction < 0)
        {
            spriteRenderer.flipX = true;

        }
        else if (direction > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    void ResetJump()
    {
        isGrounded = CheckGrounded();
        jumpRecover = false;
        jumping = false;
        attacking = false;
    }

    public void TakeDamage(int damage)
    {
        // if (attacking)
        // {
        //     return;
        // }
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
                animator.Play("warg_dead", 0, 0f);
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
        bool matchesGroundTag = collider.CompareTag(groundTag);

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

        animator.SetBool("IsCharging", charging);
        animator.SetBool("IsChargeRecovering", chargeRecover);
        animator.SetBool("IsJumpRecovering", jumpRecover);
        animator.SetBool("IsJumping", jumping);
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

        if (spawnMeat)
        {
            Instantiate(meatPrefab, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), Quaternion.identity);
            spawnMeat = false;
        }
        yield return new WaitForSeconds(deadDisableDelay);
        gameObject.SetActive(false);
    }
}

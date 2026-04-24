using UnityEngine;
using System;

public class DeerBehavior : MonoBehaviour
{
    Rigidbody2D rigidBody;
    BoxCollider2D bodyCollider;
    SpriteRenderer spriteRenderer;
    Transform playerTransform;
    Sprite[] idleFrames;
    Sprite[] walkFrames;

    bool turnAround;
    bool turning;
    int patrolDirection;
    bool isDead;
    bool wasMoving;
    float animationTimer;
    int animationFrameIndex;

    [Header("Combat")]
    [SerializeField] int health = 3;
    [SerializeField] float attackCooldown = 0.5f;
    [SerializeField] float hitCooldown = 0.1f;
    [SerializeField] float deadDisableDelay = 0.8f;

    float nextAttackTime;
    float nextHitTime;

    [Header("Behavior")]
    [SerializeField] float patrolSpeed = 1f;
    [SerializeField] float reactionMoveSpeed = 1.4f;
    [SerializeField] float alertDistance = 3f;
    [SerializeField] float counterattackDistance = 1.2f;
    [SerializeField] float turnDelay = 1f;

    [Header("Animation")]
    [SerializeField] float idleAnimationFps = 5f;
    [SerializeField] float walkAnimationFps = 8f;
    [SerializeField] string idleResourcePath = "Deer/elk_idle";
    [SerializeField] string walkResourcePath = "Deer/elk_walk";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        patrolDirection = 1;
        turnAround = false;
        turning = false;
        isDead = false;

        RefreshPlayerReference();
        EnsureHurtBox();
        LoadAnimationFrames();
        ApplyCurrentAnimationFrame(forceReset: true);
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseController.IsGamePaused)
        {
            if (rigidBody != null)
            {
                rigidBody.linearVelocity = Vector2.zero;
            }
            return;
        }
        if (isDead)
        {
            if (rigidBody != null)
            {
                rigidBody.linearVelocity = Vector2.zero;
            }
            return;
        }

        if (playerTransform == null)
        {
            RefreshPlayerReference();
        }

        if (turning)
        {
            rigidBody.linearVelocityX = 0f;
        }
        else
        {
            ApplyMovementByDistance();
            turnCheck();
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Time.time < nextHitTime ? Color.red : Color.white;
        }

        if (rigidBody.linearVelocityX < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (rigidBody.linearVelocityX > 0)
        {
            spriteRenderer.flipX = false;
        }

        UpdateAnimation();
    }

    void turnCheck()
    {
        if (turnAround && !turning)
        {
            rigidBody.linearVelocityX = 0;
            turnAround = false;
            turning = true;
            patrolDirection = -patrolDirection;
            Invoke(nameof(turn), turnDelay);
        }
    }

    void turn()
    {
        turning = false;
    }

    void ApplyMovementByDistance()
    {
        float moveDirection = patrolDirection;
        float moveSpeed = patrolSpeed;

        if (playerTransform != null)
        {
            float deltaX = playerTransform.position.x - transform.position.x;
            float distanceToPlayer = Mathf.Abs(deltaX);
            float playerDir = Mathf.Sign(deltaX);

            if (distanceToPlayer <= counterattackDistance)
            {
                // Counterattack range: move toward player.
                moveDirection = playerDir;
                moveSpeed = reactionMoveSpeed;
            }
            // else if (distanceToPlayer <= alertDistance)
            // {
            //     // Alert range only: move away from player.
            //     moveDirection = -playerDir;
            //     moveSpeed = reactionMoveSpeed;
            // }
        }

        rigidBody.linearVelocityX = moveDirection * moveSpeed;
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
            turning = false;
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

    void LoadAnimationFrames()
    {
        idleFrames = Resources.LoadAll<Sprite>(idleResourcePath);
        walkFrames = Resources.LoadAll<Sprite>(walkResourcePath);
        Array.Sort(idleFrames, (a, b) => string.CompareOrdinal(a.name, b.name));
        Array.Sort(walkFrames, (a, b) => string.CompareOrdinal(a.name, b.name));

        if (idleFrames.Length == 0)
        {
            Debug.LogWarning($"DeerBehavior on {name} could not load idle frames from Resources/{idleResourcePath}.");
        }

        if (walkFrames.Length == 0)
        {
            Debug.LogWarning($"DeerBehavior on {name} could not load walk frames from Resources/{walkResourcePath}.");
        }
    }

    void UpdateAnimation()
    {
        if (spriteRenderer == null || isDead)
        {
            return;
        }

        bool isMoving = Mathf.Abs(rigidBody.linearVelocityX) > 0.01f && !turning;
        if (isMoving != wasMoving)
        {
            ApplyCurrentAnimationFrame(forceReset: true);
            return;
        }

        Sprite[] activeFrames = isMoving ? walkFrames : idleFrames;
        float activeFps = isMoving ? walkAnimationFps : idleAnimationFps;
        if (activeFrames == null || activeFrames.Length == 0 || activeFps <= 0f)
        {
            return;
        }

        animationTimer += Time.deltaTime;
        float frameDuration = 1f / activeFps;
        if (animationTimer < frameDuration)
        {
            return;
        }

        animationTimer -= frameDuration;
        animationFrameIndex = (animationFrameIndex + 1) % activeFrames.Length;
        spriteRenderer.sprite = activeFrames[animationFrameIndex];
    }

    void ApplyCurrentAnimationFrame(bool forceReset)
    {
        bool isMoving = rigidBody != null && Mathf.Abs(rigidBody.linearVelocityX) > 0.01f && !turning;
        Sprite[] activeFrames = isMoving ? walkFrames : idleFrames;
        if (activeFrames == null || activeFrames.Length == 0)
        {
            wasMoving = isMoving;
            return;
        }

        if (forceReset)
        {
            animationTimer = 0f;
            animationFrameIndex = 0;
        }

        spriteRenderer.sprite = activeFrames[Mathf.Clamp(animationFrameIndex, 0, activeFrames.Length - 1)];
        wasMoving = isMoving;
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

    void RefreshPlayerReference()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }
}

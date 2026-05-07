using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;

    [Header("Movement")]
    [SerializeField] float speed = 5.0f;
    [SerializeField] LayerMask groundLayers = ~0;
    [SerializeField] float groundCheckDistance = 0.08f;

    [Header("Health")]
    private static int savedHealth = -1;
    [SerializeField] int maxHealth = 5;
    [SerializeField] float damageCooldown = 0.2f;
    public int playerHealth;

    [Header("Attack")]
    [SerializeField] float attackCooldown = 0.5f;
    [SerializeField] float attackLockDuration = 0.2f;
    float nextAttackTime;
    float attackEndTime;
    bool isAttacking;

    [Header("References")]
    [SerializeField] Animator player_animator;
    [SerializeField] ParticleSystem smokeFX;

    Rigidbody2D rigidBody;
    Collider2D bodyCollider;
    SpriteRenderer spriteRenderer;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction attackAction;

    int spawnPointNum;
    float nextDamageTime;
    GameObject spawnPoint;
    private bool playFootsteps = false;
    [SerializeField] float footstepInterval = 0.4f;
    float footstepTimer;
    bool isInDialogueScene;

    private void Awake()
    {
        // Handle play-mode without domain reloads: if a stale instance exists, clear it.
        if (instance != null && instance == null) instance = null;

        if (instance != null && instance != this)
        {
            // Another persistent player already exists; remove this duplicate.
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        if (bodyCollider == null)
        {
            bodyCollider = GetComponentInChildren<Collider2D>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer missing on PlayerController GameObject.");
        }

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        attackAction = InputSystem.actions.FindAction("Attack");
        playerHealth = maxHealth;

        if (savedHealth == -1)
        {
            playerHealth = maxHealth;
        }
        else
        {
            playerHealth = savedHealth;
        }
        UpdateHealthBarUI();

        ApplySceneState(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySceneState(scene.name);
        UpdateHealthBarUI();
    }
    private void UpdateHealthBarUI()
    {
        HealthBarController.Instance?.UpdateHealthBar(playerHealth);
    }

    void Update()
    {
        if (isInDialogueScene)
        {
            return;
        }


        //if the game ends, disable
        if (SceneManager.GetActiveScene().name == "gameover" || SceneManager.GetActiveScene().name == "winandrestart")
        {
            Destroy(gameObject);
        }
        //if the game is paused, stop all movement and animations
        if (PauseController.IsGamePaused)
        {
            rigidBody.linearVelocityX = 0f;
            player_animator.SetBool("IsRunning", false);
            return;
        }
        // Game over check
        if (playerHealth == 0)
        {
            savedHealth = -1;
            ScoreManager.Calculate(FindObjectOfType<InventoryController>());
            Destroy(gameObject);
            SceneManager.LoadScene("gameover");
            return;
        }

        // Damage flash
        if (spriteRenderer != null)
            spriteRenderer.color = (Time.time < nextDamageTime) ? Color.red : Color.white;

        // Movement (horizontal)
        if (isAttacking)
        {
            rigidBody.linearVelocityX = 0f;
        }
        else if (moveAction != null)
        {
            rigidBody.linearVelocityX = moveAction.ReadValue<Vector2>().x * speed;
        }

        if (isAttacking && Time.time >= attackEndTime)
        {
            isAttacking = false;
        }

        // Flipping sprite based on movement direction
        if (rigidBody.linearVelocityX < 0)
            spriteRenderer.flipX = false;
        else if (rigidBody.linearVelocityX > 0)
            spriteRenderer.flipX = true;

        // Jumping (only when grounded)
        if (jumpAction != null && jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            rigidBody.AddForce(new Vector2(0, speed * 2f), ForceMode2D.Impulse);
            if (player_animator != null)
            {
                player_animator.SetTrigger("Jump");
            }
            smokeFX?.Play();
        }

        // Attack input (supports both new Input System and legacy KeyCode.J)
        bool attackPressed = false;
        if (attackAction != null)
            attackPressed = attackAction.WasPressedThisFrame();
        else
            attackPressed = Input.GetKeyDown(KeyCode.J);

        if (attackPressed && Time.time >= nextAttackTime)
            Attack();

        // Animation: running state
        if (player_animator != null)
            player_animator.SetBool("IsRunning", rigidBody.linearVelocityX != 0);

        // Smoke FX: play while moving, stop when idle
        if (smokeFX != null)
        {
            if (rigidBody.linearVelocityX != 0)
            {
                if (!smokeFX.isPlaying) smokeFX.Play();
            }
            else if (IsGrounded())
            {
                smokeFX.Stop();
            }
        }

        // Footstep sounds
        bool isMoving = rigidBody.linearVelocityX != 0;
        if (isMoving && IsGrounded())
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                if (IsInWater() == true)
                {
                    Debug.Log("water!");
                    SoundEffectManager.Play("waterFootstep");
                }
                else
                {
                    SoundEffectManager.Play("footstep");

                }
                footstepTimer = footstepInterval;
            }
        }

        else
        {
            footstepTimer = 0f;
        }
    }

    void Attack()
    {
        if (player_animator == null) return;

        isAttacking = true;
        attackEndTime = Time.time + attackLockDuration;
        player_animator.SetTrigger("Attack");
        nextAttackTime = Time.time + attackCooldown;
    }

    public int getPlayerHealth() => playerHealth;

    public void TakeDamage(int damage)
    {
        if (Time.time < nextDamageTime) return;

        playerHealth = Mathf.Max(0, playerHealth - damage);
        nextDamageTime = Time.time + damageCooldown;

        SoundEffectManager.Play("hurt");
        savedHealth = playerHealth;
        UpdateHealthBarUI();
    }

    public void Heal(int amount)
    {
        playerHealth = Mathf.Min(maxHealth, playerHealth + amount);
        savedHealth = playerHealth;
        UpdateHealthBarUI();
    }

    public void HealFull()
    {
        playerHealth = maxHealth;
        savedHealth = playerHealth;
        UpdateHealthBarUI();
    }

    bool IsGrounded()
    {
        if (bodyCollider == null)
        {
            return false;
        }

        Bounds bounds = bodyCollider.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y + 0.02f);
        Vector2 size = new Vector2(bounds.size.x * 0.85f, 0.06f);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, size, 0f, Vector2.down, groundCheckDistance, groundLayers.value);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hitCollider = hits[i].collider;
            if (hitCollider == null || hitCollider.isTrigger)
            {
                continue;
            }

            if (hitCollider.attachedRigidbody == rigidBody)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    bool IsInWater()
    {
        if (bodyCollider == null)
        {
            return false;
        }

        Bounds bounds = bodyCollider.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y + 0.02f);
        Vector2 size = new Vector2(bounds.size.x * 0.85f, 0.06f);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, size, 0f, Vector2.down, groundCheckDistance, groundLayers.value);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hitCollider = hits[i].collider;
            if (hitCollider == null || hitCollider.isTrigger)
            {
                continue;
            }

            if (hitCollider.attachedRigidbody == rigidBody)
            {
                continue;
            }
            if (hitCollider.gameObject.CompareTag("Water"))
            {
                return true;
            }


        }

        return false;
    }

    void setSpawnPoint()
    {
        SpawnPointHandler spawnHandler = SpawnPointHandler.Instance;
        if (spawnHandler == null)
        {
            Debug.LogError("SpawnPointHandler instance not found");
            return;
        }

        spawnPointNum = spawnHandler.getSpawnPoint();
        spawnPoint = GameObject.Find("SpawnPoint" + spawnPointNum);

        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint" + spawnPointNum + " not found");
            return;
        }

        SpawnPoint spawn = spawnPoint.GetComponent<SpawnPoint>();
        if (spawn == null)
        {
            Debug.LogError("SpawnPoint component not found on SpawnPoint" + spawnPointNum);
            return;
        }

        transform.position = spawn.getPosition();
    }

    void ApplySceneState(string sceneName)
    {
        bool isDialogue = DayManager.Instance != null
            ? DayManager.Instance.IsDialogueScene(sceneName)
            : sceneName == "Dialogue";

        isInDialogueScene = isDialogue;

        if (rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        if (isDialogue)
        {
            if (rigidBody != null)
            {
                rigidBody.linearVelocity = Vector2.zero;
                rigidBody.simulated = false;
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            if (bodyCollider != null)
            {
                bodyCollider.enabled = false;
            }

            return;
        }

        if (rigidBody != null)
        {
            rigidBody.simulated = true;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        if (bodyCollider != null)
        {
            bodyCollider.enabled = true;
        }

        setSpawnPoint();
    }
}

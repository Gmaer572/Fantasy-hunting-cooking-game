using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 5.0f;

    [Header("Health")]
    int maxHealth = 5;
    [SerializeField] float damageCooldown = 0.2f;
    public int playerHealth;

    [Header("Attack")]
    [SerializeField] float attackCooldown = 0.5f;
    float nextAttackTime;

    [Header("References")]
    [SerializeField] Animator player_animator;
    [SerializeField] HealthBarController healthBar;

    Rigidbody2D rigidBody;
    SpriteRenderer spriteRenderer;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction attackAction;   // New Input Action for attack

    int spawnPointNum;
    float nextDamageTime;
    GameObject spawnPoint;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        attackAction = InputSystem.actions.FindAction("Attack"); 
        playerHealth = maxHealth;

        if (healthBar == null)
            healthBar = FindFirstObjectByType<HealthBarController>();

        if (healthBar != null)
            healthBar.UpdateHealthBar(playerHealth);
        else
            Debug.LogError("HealthBarController not found");

        setSpawnPoint();
    }

    void Update()
    {
        // Game over check
        if (playerHealth == 0)
            SceneManager.LoadScene("gameover");

        // Damage flash
        spriteRenderer.color = (Time.time < nextDamageTime) ? Color.red : Color.white;

        // Movement (horizontal)
        if (moveAction != null)
            rigidBody.linearVelocityX = moveAction.ReadValue<Vector2>().x * speed;

        // Flipping sprite based on movement direction
        if (rigidBody.linearVelocityX < 0)
            spriteRenderer.flipX = false;
        else if (rigidBody.linearVelocityX > 0)
            spriteRenderer.flipX = true;

        // Jumping (only when grounded)
        if (rigidBody.linearVelocityY == 0 && jumpAction != null && jumpAction.WasPressedThisFrame())
            rigidBody.AddForce(new Vector2(0, speed), ForceMode2D.Impulse);

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
    }

    void Attack()
    {
        if (player_animator == null) return;

        player_animator.SetTrigger("Attack");
        nextAttackTime = Time.time + attackCooldown;
    }

    public int getPlayerHealth() => playerHealth;

    public void TakeDamage(int damage)
    {
        if (Time.time < nextDamageTime) return;

        playerHealth = Mathf.Max(0, playerHealth - damage);
        nextDamageTime = Time.time + damageCooldown;

        if (healthBar != null)
            healthBar.UpdateHealthBar(playerHealth);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        SceneTransition transition = collision.GetComponent<SceneTransition>();
        if (transition != null)
            SceneManager.LoadScene(transition.getSceneToLoad(), LoadSceneMode.Single);
    }

    void setSpawnPoint()
    {
        GameObject handler = GameObject.Find("SpawnPointHandler");
        if (handler == null)
        {
            Debug.LogError("SpawnPointHandler not found");
            return;
        }

        SpawnPointHandler spawnHandler = handler.GetComponent<SpawnPointHandler>();
        if (spawnHandler == null)
        {
            Debug.LogError("SpawnPointHandler component not found");
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
}
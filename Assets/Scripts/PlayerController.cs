using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float speed = 5.0f;

    int maxHealth = 5;

    [SerializeField]
    float damageCooldown = 0.2f;

    [SerializeField] Animator player_animator;
    [SerializeField] HealthBarController healthBar;

    public int playerHealth;

    Rigidbody2D rigidBody;
    InputAction moveAction;
    InputAction jumpAction;

    int spawnPointNum;
    SpriteRenderer spriteRenderer;

    [SerializeField]
    Sprite idleSprite;

    float nextDamageTime;
    GameObject spawnPoint;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        spriteRenderer.sprite = idleSprite;
        playerHealth = maxHealth;

        if (healthBar == null)
        {
            healthBar = FindFirstObjectByType<HealthBarController>();
        }

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(playerHealth);
        }
        else
        {
            Debug.LogError("HealthBarController not found");
        }

        setSpawnPoint();
    }

    void Update()
    {
        if (playerHealth == 0)
        {
            SceneManager.LoadScene("gameover");
        }

        if (Time.time < nextDamageTime)
        {
            spriteRenderer.color = Color.red;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }

        if (moveAction != null)
        {
            rigidBody.linearVelocityX = moveAction.ReadValue<Vector2>().x * speed;
        }

        if (rigidBody.linearVelocityX < 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (rigidBody.linearVelocityX > 0)
        {
            spriteRenderer.flipX = true;
        }

        if (rigidBody.linearVelocityY == 0)
        {
            if (jumpAction != null && jumpAction.WasPressedThisFrame())
            {
                rigidBody.AddForce(new Vector2(0, speed), ForceMode2D.Impulse);
            }
        }

        if (player_animator != null)
        {
            player_animator.SetBool("IsRunning", rigidBody.linearVelocityX != 0);
        }
    }

    public int getPlayerHealth()
    {
        return playerHealth;
    }

    public void TakeDamage(int damage)
    {
        if (Time.time < nextDamageTime)
        {
            return;
        }

        playerHealth = Mathf.Max(0, playerHealth - damage);
        nextDamageTime = Time.time + damageCooldown;

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(playerHealth);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        SceneTransition transition = collision.GetComponent<SceneTransition>();
        if (transition == null)
        {
            return;
        }

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
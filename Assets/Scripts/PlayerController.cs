using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float speed = 5.0f;
    public float attackDuration = 0.3f;
    int maxHealth = 5;
    [SerializeField]
    float damageCooldown = 0.2f;
    [SerializeField]
    float attackAnimationFps = 12f;
    [SerializeField]
    Sprite[] attackFrames;

    [SerializeField] Animator player_animator;
    [SerializeField] HealthBarController healthBar;
    public int playerHealth;
    Rigidbody2D rigidBody;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction attackAction;

    int spawnPointNum;
    SpriteRenderer spriteRenderer;
    [SerializeField]
    Sprite idleSprite;
    float nextDamageTime;
    bool isAttacking;

    GameObject spawnPoint;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        attackAction = InputSystem.actions.FindAction("Attack");
        spriteRenderer.sprite = idleSprite;
        playerHealth = maxHealth;
        TryLoadAttackFramesFromAsset();
        attackDuration = CalculateAttackDuration();

        //HEALTH BAR TEST
        healthBar = FindFirstObjectByType<HealthBarController>();
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(playerHealth);
        }
        // float height = Camera.main.orthographicSize * 2;
        // float width = height * Camera.main.aspect;

        // Debug.Log("Camera Width: " + width);
        // Debug.Log("Camera Height: " + height);
        setSpawnPoint();
    }

    // Update is called once per frame
    void Update()
    {

        rigidBody.linearVelocityX = moveAction.ReadValue<Vector2>().x * speed;
        if (rigidBody.linearVelocityX < 0)
        {
            spriteRenderer.flipX = false;
        }
        if (rigidBody.linearVelocityX > 0)
        {
            spriteRenderer.flipX = true;

        }

        if (rigidBody.linearVelocityY == 0) //check for already falling/jumping; reprogram later to check if grounded
        {
            if (jumpAction.WasPressedThisFrame())
            {
                rigidBody.AddForce(new Vector2(0, speed), ForceMode2D.Impulse);
            }
        }

        if (!isAttacking && rigidBody.linearVelocityX != 0) //check for movement
        {
            player_animator.SetBool("IsRunning", true);
        }
        else
        {
            player_animator.SetBool("IsRunning", false);
        }

        if (attackAction != null && attackAction.WasPressedThisFrame() && !isAttacking)
        {
            StartCoroutine(PlayAttackAnimation());
        }

    }

    IEnumerator PlayAttackAnimation()
    {
        isAttacking = true;
        player_animator.enabled = false;

        if (attackFrames != null && attackFrames.Length > 0)
        {
            float frameDuration = 1f / Mathf.Max(1f, attackAnimationFps);
            for (int i = 0; i < attackFrames.Length; i++)
            {
                Sprite currentFrame = attackFrames[i];
                spriteRenderer.sprite = currentFrame;
                yield return new WaitForSeconds(frameDuration);
            }
        }
        else
        {
            yield return new WaitForSeconds(attackDuration);
        }

        spriteRenderer.sprite = idleSprite;
        player_animator.enabled = true;
        isAttacking = false;
    }

    float CalculateAttackDuration()
    {
        if (attackFrames == null || attackFrames.Length == 0)
        {
            return attackDuration;
        }

        return attackFrames.Length / Mathf.Max(1f, attackAnimationFps);
    }

    void TryLoadAttackFramesFromAsset()
    {
#if UNITY_EDITOR
        if (attackFrames != null && attackFrames.Length > 0)
        {
            return;
        }

        Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Characters/player/player_attack.aseprite");
        if (assets == null || assets.Length == 0)
        {
            return;
        }

        System.Collections.Generic.List<Sprite> sprites = new System.Collections.Generic.List<Sprite>();
        for (int i = 0; i < assets.Length; i++)
        {
            if (assets[i] is Sprite sprite)
            {
                sprites.Add(sprite);
            }
        }

        sprites.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
        if (sprites.Count > 0)
        {
            attackFrames = sprites.ToArray();
        }
#endif
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

        // Update the health bar UI
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
        spawnPointNum = GameObject.Find("SpawnPointHandler").GetComponent<SpawnPointHandler>().getSpawnPoint();
        spawnPoint = GameObject.Find("SpawnPoint" + spawnPointNum);
        transform.position = spawnPoint.GetComponent<SpawnPoint>().getPosition();
    }

}

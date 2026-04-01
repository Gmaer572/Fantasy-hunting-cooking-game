using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float speed = 5.0f;
    public float attackDuration = 0.05f;
    [SerializeField]
    int maxHealth = 3;
    [SerializeField]
    float damageCooldown = 0.2f;
    [SerializeField] Animator player_animator;

    public int playerHealth;
    Rigidbody2D rigidBody;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction attackAction;

    SpriteRenderer spriteRenderer;
    [SerializeField]
    Sprite idleSprite;
    [SerializeField]
    Sprite attackSprite;
    float nextDamageTime;





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

        // float height = Camera.main.orthographicSize * 2;
        // float width = height * Camera.main.aspect;

        // Debug.Log("Camera Width: " + width);
        // Debug.Log("Camera Height: " + height);
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

        if(rigidBody.linearVelocityX != 0) //check for movement
        {
            player_animator.SetBool("IsRunning", true);
        }
        else
        {
            player_animator.SetBool("IsRunning", false);
        }

        if (attackAction != null && attackAction.WasPressedThisFrame())
        {
            spriteRenderer.sprite = attackSprite;
            CancelInvoke(nameof(resetSprite));
            Invoke(nameof(resetSprite), attackDuration);
        }

    }
    void resetSprite()
    {
        spriteRenderer.sprite = idleSprite;
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

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        SceneTransition transition = collision.GetComponent<SceneTransition>();
   //     if (collision.CompareTag("Transition"))
   {
            SceneManager.LoadScene(transition.getSceneToLoad(), LoadSceneMode.Single);

        }
    }

}

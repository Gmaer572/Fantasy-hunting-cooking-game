using System;
using UnityEngine;

public class SlimeBehaviour : MonoBehaviour
{
    BoxCollider2D boxCollider;
    Rigidbody2D rigidBody;

    Boolean jumping;

    [SerializeField]
    int health = 5;
    [SerializeField]
    int contactDamage = 1;
    [SerializeField]
    float attackCooldown = 0.5f;
    [SerializeField]
    float hitCooldown = 0.1f;
    float nextAttackTime;
    float nextHitTime;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        if (rigidBody == null)
        {
            rigidBody = GetComponentInParent<Rigidbody2D>();
        }
        jumping = true;

    }

    void Update()
    {
        if (jumping == false)
        {
            Invoke("SlimeJump", UnityEngine.Random.Range(0.5f, 1.0f));

            jumping = true;
        }

    }

    /*void DisableCollider()
    {
        boxCollider.enabled = false;
    }*/

    void SlimeJump()
    {
        jumping = true;
        float xSpeed = 0;
        int jumpLeftOrRight = UnityEngine.Random.Range(1, 3);
        if (jumpLeftOrRight == 1)
        {
            xSpeed = UnityEngine.Random.Range(-5.0f, -3.0f);
        }
        else if (jumpLeftOrRight == 2)
        {
            xSpeed = UnityEngine.Random.Range(3.0f, 5.0f);
        }
        rigidBody.AddForce(new Vector2(xSpeed, UnityEngine.Random.Range(3.0f, 5.0f)), ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") && rigidBody.linearVelocityY == 0)
        {
            Invoke("ResetJump", 1f);

        }
        TryDamagePlayer(collision.collider);

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        TryTakeDamage(collision);

    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") && rigidBody.linearVelocityY == 0)
        {
            jumping = true;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        TryDamagePlayer(collision.collider);
    }

    void ResetJump()
    {
        jumping = false;
    }

    void TryTakeDamage(Collider2D collider)
    {
        if (Time.time < nextHitTime)
        {
            return;
        }

        AttackController attackController = collider.GetComponent<AttackController>();
        if (attackController == null)
        {
            return;
        }

        nextHitTime = Time.time + hitCooldown;
        health = Mathf.Max(0, health - attackController.GetDamage());

        if (health == 0)
        {
            gameObject.SetActive(false);
        }

    }

    void TryDamagePlayer(Collider2D collider)
    {
        if (Time.time < nextAttackTime)
        {
            return;
        }

        PlayerController playerController = collider.GetComponent<PlayerController>();
        if (playerController == null)
        {
            playerController = collider.GetComponentInParent<PlayerController>();
        }

        if (playerController == null)
        {
            return;
        }

        nextAttackTime = Time.time + attackCooldown;
        playerController.TakeDamage(contactDamage);
    }
}

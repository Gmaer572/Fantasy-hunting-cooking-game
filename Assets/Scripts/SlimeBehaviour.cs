using System;
using UnityEngine;

public class SlimeBehaviour : MonoBehaviour
{
    Rigidbody2D rigidBody;
    SpriteRenderer renderer;

    Boolean jumping;

    [SerializeField]
    int health = 5;
    [SerializeField]
    float attackCooldown = 0.5f;
    [SerializeField]
    float hitCooldown = 0.1f;
    float nextAttackTime;
    float nextHitTime;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
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
        if (Time.time < nextHitTime)
        {
            renderer.color = Color.red;
        }
        else
        {
            renderer.color = Color.white;
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
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") && rigidBody.linearVelocityY == 0)
        {
            jumping = true;
        }
    }

    void ResetJump()
    {
        jumping = false;
    }

    public void TakeDamage(int damage)
    {
        if (Time.time < nextHitTime)
        {
            return;
        }

        nextHitTime = Time.time + hitCooldown;
        health = Mathf.Max(0, health - damage);

        if (health == 0)
        {
            gameObject.SetActive(false);
        }

    }

    public void TryDamagePlayer(PlayerController playerController, int damage)
    {
        if (Time.time < nextAttackTime)
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
}

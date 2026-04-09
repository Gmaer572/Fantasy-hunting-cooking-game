using UnityEngine;
using UnityEngine.Rendering;

public class DeerBehavior : MonoBehaviour
{

    Rigidbody2D rigidBody;
    SpriteRenderer spriteRenderer;

    GameObject attack;
    bool turnAround;
    int speed;
    int tempSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        speed = 1;
        tempSpeed = speed;
        turnAround = false;
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.linearVelocityX = speed;
        turnCheck();

        if (speed < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (speed > 0)
        {
            spriteRenderer.flipX = false;
        }

    }

    void turnCheck()
    {
        if (turnAround == true)
        {
            rigidBody.linearVelocityX = 0;
            turnAround = false;
            tempSpeed = speed;
            speed = 0;
            Invoke(nameof(turn), 1.0f);

        }
    }

    void turn()
    {
        speed = -tempSpeed;

    }
    public void setTurn(bool turn)
    {
        turnAround = turn;
    }

    public bool getTurn()
    {
        return turnAround;
    }
}

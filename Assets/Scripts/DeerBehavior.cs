using UnityEngine;
using UnityEngine.Rendering;

public class DeerBehavior : MonoBehaviour
{

    Rigidbody2D rigidBody;
    SpriteRenderer spriteRenderer;
    bool turnAround;
    int speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        speed = 2;
        turnAround = false;
    }

    // Update is called once per frame
    void Update()
    {
        turnCheck();
        rigidBody.linearVelocityX = speed;
        if (speed < 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    void turnCheck()
    {
        if (turnAround == true)
        {
            speed = -speed;
            Debug.Log(speed);
            turnAround = false;

        }
    }
    public void setTurn(bool turn)
    {
        turnAround = turn;
        Debug.Log(turnAround);
    }

    public bool getTurn()
    {
        return turnAround;
    }
}

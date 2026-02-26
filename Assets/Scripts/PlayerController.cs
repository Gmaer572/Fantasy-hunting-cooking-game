using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float speed = 5.0f;
    Rigidbody2D rigidBody;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction attackAction;

    SpriteRenderer spriteRenderer;
    [SerializeField]
    Sprite idleSprite;
    [SerializeField]
    Sprite attackSprite;





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        attackAction = InputSystem.actions.FindAction("Attack");
        spriteRenderer.sprite = idleSprite;


    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.linearVelocityX = moveAction.ReadValue<Vector2>().x * speed;

        if (rigidBody.linearVelocityY == 0) //check for already falling/jumping
        {
            if (jumpAction.WasPressedThisFrame())
            {
                rigidBody.AddForce(new Vector2(0, speed), ForceMode2D.Impulse);
            }
        }

        if (attackAction.WasPressedThisFrame())
        {
            spriteRenderer.sprite = attackSprite;
            Invoke("resetSprite", 2);
        }
    }
    void resetSprite()
    {
        spriteRenderer.sprite = idleSprite;
    }
}

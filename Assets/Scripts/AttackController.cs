
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class AttackController : MonoBehaviour
{
    InputAction attackAction;
    BoxCollider2D boxCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (attackAction.WasPressedThisFrame())
        {
            boxCollider.enabled = true;
            Invoke("disableCollider", 2);
        }
    }

    void disableCollider()
    {
        boxCollider.enabled = false;
    }
}

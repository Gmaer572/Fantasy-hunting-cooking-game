using System.Numerics;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class DeerLookBox : MonoBehaviour
{

    BoxCollider2D boxCollider;
    UnityEngine.Vector2 offsetLeft;
    UnityEngine.Vector2 offsetRight;

    SpriteRenderer parentRenderer;
    DeerBehavior deerBehavior;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        deerBehavior = GetComponentInParent<DeerBehavior>();
        offsetLeft = new UnityEngine.Vector2(-1f, transform.localPosition.y);
        offsetRight = new UnityEngine.Vector2(-0.5f, transform.localPosition.y);
        parentRenderer = GetComponentInParent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = parentRenderer.flipX ? offsetLeft : offsetRight;

    }

    void OnTriggerExit2D(UnityEngine.Collider2D collider)
    {
        if (collider.CompareTag("Ground") || collider.CompareTag("DeerBoundary"))
        {
            Debug.Log("no ground detected");
            deerBehavior.setTurn(true);
        }
    }
}

using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class DeerLookBox : MonoBehaviour
{

    BoxCollider2D boxCollider;
    float offsetLeft;
    float offsetRight;
    DeerBehavior deerBehavior;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        deerBehavior = GetComponentInParent<DeerBehavior>();
        offsetLeft = -0.5f;
        offsetRight = -1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerExit2D(UnityEngine.Collider2D collider)
    {
        //if (!deerBehavior.getTurn())
        //{
        Debug.Log("no ground detected");
        deerBehavior.setTurn(true);
        transform.localPosition = new Vector2(((transform.localPosition.x) / 2), (transform.localPosition.y));
        //}
    }
}

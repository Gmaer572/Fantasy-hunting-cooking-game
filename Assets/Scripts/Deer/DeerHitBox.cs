using UnityEngine;

public class DeerHitBox : MonoBehaviour
{

    Vector2 attackOffsetLeft;
    Vector2 attackOffsetRight;

    SpriteRenderer parentRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackOffsetLeft = new Vector2(-0.37f, 0f);
        attackOffsetRight = new Vector2(.21f, 0f);
        parentRenderer = GetComponentInParent<SpriteRenderer>();
        transform.localPosition = attackOffsetRight;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = parentRenderer.flipX ? attackOffsetLeft : attackOffsetRight;
    }


}

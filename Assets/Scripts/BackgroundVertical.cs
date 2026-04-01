using UnityEngine;

public class BackgroundVertical : MonoBehaviour
{
    private Vector2 startPos;  
    private float lengthX;
    private float lengthY;

    public GameObject cam;
    public float parallaxEffectX;
    public float parallaxEffectY;

    void Start()
    {
        startPos = transform.position;

        var bounds = GetComponent<SpriteRenderer>().bounds.size;
        lengthX = bounds.x;
        lengthY = bounds.y;
    }

    void Update()
    {
 
        float distX = cam.transform.position.x * parallaxEffectX;
        float moveX = cam.transform.position.x * (1 - parallaxEffectX);


        float distY = cam.transform.position.y * parallaxEffectY;
        float moveY = cam.transform.position.y * (1 - parallaxEffectY);


        transform.position = new Vector3(
            startPos.x + distX,
            startPos.y + distY,
            transform.position.z
        );

        if (moveX > startPos.x + lengthX)
            startPos.x += lengthX;
        else if (moveX < startPos.x - lengthX)
            startPos.x -= lengthX;

        if (moveY > startPos.y + lengthY)
            startPos.y += lengthY;
        else if (moveY < startPos.y - lengthY)
            startPos.y -= lengthY;
    }
}
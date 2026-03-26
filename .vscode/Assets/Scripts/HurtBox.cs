using UnityEngine;


public class HurtBox : MonoBehaviour
{
    int health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = 3;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attack"))
        {
            health--;

        }
    }
}

using System;
using Unity.VisualScripting;
using UnityEngine;


public class EnemyHurtBox : MonoBehaviour
{
    [SerializeField]
    int health;
    Boolean hittable;

    [SerializeField]
    int iFrameDuration;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hittable = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.CompareTag("Attack") && hittable)
        {
            health--;
            hittable = false;
            Invoke("removeIFrames", iFrameDuration);
        }

    }
    //Invincibility frames, or "I-Frames", are the frames where an object cannot be hit.
    //This is in order to prevent, say, taking damage every frame where it overlaps with a hitbox, instead of just
    //taking one damage because it was hit once.
    void removeIframes()
    {
        hittable = true;
    }
}

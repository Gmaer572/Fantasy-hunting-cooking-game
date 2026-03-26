using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    BoxCollider2D boxCollider;
    Rigidbody2D parentBody;

    [SerializeField]
    float attackDuration = 0.5f;
    [SerializeField]
    public static int enemyHealth = 10;

    public bool attackLeft = false;
    public bool attackRight = true;



    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
        parentBody = GetComponentInParent<Rigidbody2D>();
    }

    void Update()
    {
        if (attackLeft)
        {
            transform.localPosition = new Vector3(-0.024f, 0.5250067f, 0f);
        }

        if (attackRight)
        {
            transform.localPosition = new Vector3(0.1738255f, 0.5250067f, 0f);
        }
    }

    public void StartAttack(bool faceLeft)
    {
        attackLeft = faceLeft;
        attackRight = !faceLeft;

        boxCollider.enabled = true;
        Invoke("DisableCollider", attackDuration);
    }

    void DisableCollider()
    {
        boxCollider.enabled = false;
    }
}
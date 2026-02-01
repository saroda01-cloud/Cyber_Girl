using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private Collider2D platformCollider;
    private Collider2D playerCollider;
    private Rigidbody2D playerRb;

    public float tolerance = 0.05f;
    private bool isIgnoring = true;   // 기본: 무시 상태

    void Start()
    {
        platformCollider = GetComponent<Collider2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerCollider = player.GetComponent<Collider2D>();
        playerRb = player.GetComponent<Rigidbody2D>();

        // 시작부터 충돌 무시
        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
    }

    void FixedUpdate()
    {
        float playerBottom = playerCollider.bounds.min.y;
        float platformTop = platformCollider.bounds.max.y;

        bool isAbove = playerBottom >= platformTop - tolerance;
        bool isFalling = playerRb.linearVelocity.y <= 0f;

        if (isAbove && isFalling)
        {
            if (isIgnoring)
            {
                Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
                isIgnoring = false;
            }
        }
        else
        {
            if (!isIgnoring)
            {
                Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
                isIgnoring = true;
            }
        }
    }
}

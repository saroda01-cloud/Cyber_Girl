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

    void Update()
    {
        float playerBottom = playerCollider.bounds.min.y;   // 발 위치
        float platformTop = platformCollider.bounds.max.y;  // 발판 윗면

        bool isAbove = playerBottom >= platformTop - tolerance;
        bool isFalling = playerRb.linearVelocity.y <= 0f;

        // 충돌 허용 조건: 위에 있음 + 아래로 이동
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
            //  나머지는 전부 통과
            if (!isIgnoring)
            {
                Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
                isIgnoring = true;
            }
        }
    }
}

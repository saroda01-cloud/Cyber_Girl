using UnityEngine;

public class MovingWall : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f; // Inspector에서 조정 가능
    [SerializeField] private bool isMoving = false; // 처음엔 멈춰있음

    [Header("Push Settings")]
    [SerializeField] private float pushForce = 10f; // 플레이어를 미는 힘

    void Update()
    {
        if (isMoving)
        {
            // 오른쪽으로 계속 이동
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }
    }

    public void StartMoving()
    {
        isMoving = true;
        Debug.Log("밀리는 벽 시작!");
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어를 오른쪽으로 밀기
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.AddForce(Vector2.right * pushForce, ForceMode2D.Force);
            }
        }
    }
}
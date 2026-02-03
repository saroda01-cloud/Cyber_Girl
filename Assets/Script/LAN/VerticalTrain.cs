using UnityEngine;

public class VerticalTrain : MonoBehaviour
{
    private float moveSpeed;
    private float targetY;
    private bool movingUp;
    private bool hasTarget = false;

    public void SetTarget(float endY, bool isMovingUp, float speed)
    {
        targetY = endY;
        movingUp = isMovingUp;
        moveSpeed = speed;
        hasTarget = true;

        Debug.Log($"[VerticalTrain] Target set! Y: {transform.position.y} → {targetY}, Speed: {moveSpeed}");
    }

    void Update()
    {
        if (!hasTarget) return;

        if (movingUp)
        {
            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);

            if (transform.position.y >= targetY)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);

            if (transform.position.y <= targetY)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by vertical train!");

            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(); // 파라미터 없이 호출
                Debug.Log("Vertical train dealt damage to player!");
            }
            else
            {
                Debug.LogError("PlayerHealth component not found on player!");
            }
        }
    }
}
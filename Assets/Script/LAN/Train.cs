using UnityEngine;

public class Train : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed = 20f;
    private float targetX;
    private bool movingRight;
    private bool hasTarget = false;

    [Header("Lights")]
    public GameObject lightLeft;  // Light_L
    public GameObject lightRight; // Light_R

    void Start()
    {
        // Light 오브젝트 자동 찾기
        if (lightLeft == null)
        {
            Transform leftTransform = transform.Find("Light_L");
            if (leftTransform != null)
            {
                lightLeft = leftTransform.gameObject;
                Debug.Log("[Train] Light_L 자동 찾기 성공");
            }
            else
            {
                Debug.LogWarning("[Train] Light_L을 찾을 수 없습니다!");
            }
        }

        if (lightRight == null)
        {
            Transform rightTransform = transform.Find("Light_R");
            if (rightTransform != null)
            {
                lightRight = rightTransform.gameObject;
                Debug.Log("[Train] Light_R 자동 찾기 성공");
            }
            else
            {
                Debug.LogWarning("[Train] Light_R을 찾을 수 없습니다!");
            }
        }

        // 처음엔 둘 다 끄기
    }

    public void SetTarget(float endX, bool isMovingRight, float speed)
    {
        targetX = endX;
        movingRight = isMovingRight;
        moveSpeed = speed;
        hasTarget = true;


        // 방향에 따라 라이트 켜기
        if (movingRight)
        {

            if (lightRight != null)
            {
                lightRight.SetActive(true);
            }

            if (lightLeft != null) lightLeft.SetActive(false);
        }
        else
        {

            if (lightLeft != null)
            {
                lightLeft.SetActive(true);
            }

            if (lightRight != null) lightRight.SetActive(false);
        }
    }

    void Update()
    {
        if (!hasTarget)
        {
            return;
        }

        if (movingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            if (transform.position.x >= targetX)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            if (transform.position.x <= targetX)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by train!");
        }
    }
}
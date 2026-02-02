using UnityEngine;

public class Train : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed = 20f; // 빠른 속도
    private float targetX;
    private bool movingRight;
    private bool hasTarget = false;

    [Header("Light")]
    public GameObject lightObject; // Light 오브젝트
    public bool lightStartOn = true; // 시작 시 켜져 있을지 여부

    void Start()
    {
        // Light 초기 상태 설정
        if (lightObject != null)
        {
            lightObject.SetActive(lightStartOn);
        }
    }

    public void SetTarget(float endX, bool isMovingRight, float speed) // 속도 파라미터 추가
    {
        targetX = endX;
        movingRight = isMovingRight;
        moveSpeed = speed; // 속도 설정
        hasTarget = true;
        Debug.Log($"[Train] Target set! Current X: {transform.position.x}, Target X: {targetX}, Moving Right: {movingRight}");
    }

    void Update()
    {
        if (!hasTarget)
        {
            Debug.LogWarning("[Train] No target set!");
            return;
        }

        if (movingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            // 목표 지점 도달하면 삭제
            if (transform.position.x >= targetX)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            // 목표 지점 도달하면 삭제
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

    // Light를 켜는 함수
    public void TurnOnLight()
    {
        if (lightObject != null)
        {
            lightObject.SetActive(true);
        }
    }

    public void TurnOffLight()
    {
        if (lightObject != null)
        {
            lightObject.SetActive(false);
        }
    }

    public void ToggleLight()
    {
        if (lightObject != null)
        {
            lightObject.SetActive(!lightObject.activeSelf);
        }
    }
}
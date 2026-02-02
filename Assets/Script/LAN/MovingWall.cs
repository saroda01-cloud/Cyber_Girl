using UnityEngine;

public class MovingWall : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private bool isMoving = false;

    [Header("Push Settings")]
    [SerializeField] private float pushForce = 10f;

    void Start()
    {
        // 자식 오브젝트들에도 충돌 감지 추가
        WallCollisionDetector[] childDetectors = GetComponentsInChildren<WallCollisionDetector>();
        if (childDetectors.Length == 0)
        {
            // 자식들에게 감지 스크립트 추가
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Collider2D>() != null)
                {
                    WallCollisionDetector detector = child.gameObject.AddComponent<WallCollisionDetector>();
                    detector.parentWall = this;
                }
            }
        }
    }

    void Update()
    {
        if (isMoving)
        {
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandlePlayerCollision(collision.gameObject);
    }

    // 외부에서도 호출 가능하도록 public으로
    public void HandlePlayerCollision(GameObject obj)
    {
        if (obj.CompareTag("Player"))
        {
            Debug.Log("플레이어가 밀리는 벽에 닿았습니다! 즉사!");

            PlayerHealth playerHealth = obj.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Die();
            }
        }
    }
}

// 자식 오브젝트용 충돌 감지 스크립트 (새 파일로 만들어도 되고, 같은 파일 안에 넣어도 됨)
public class WallCollisionDetector : MonoBehaviour
{
    [HideInInspector] public MovingWall parentWall;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (parentWall != null)
        {
            Debug.Log($"자식 {gameObject.name}이(가) 충돌 감지!");
            parentWall.HandlePlayerCollision(collision.gameObject);
        }
    }
}
using UnityEngine;

public class MovingDeadBox : MonoBehaviour
{
    [Header("이동 경로")]
    [SerializeField] private Transform[] waypoints; // 이동할 포인트들

    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool loop = true; // true면 반복, false면 왔다갔다

    private int currentWaypointIndex = 0;
    private bool movingForward = true; // 앞으로 가는지 뒤로 가는지

    void Start()
    {
        // 시작 위치를 첫 번째 웨이포인트로
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
            if (waypoints.Length > 1)
            {
                currentWaypointIndex = 1; // 다음 목표는 두 번째 포인트
            }
        }
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // 현재 목표 포인트
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;

        // 목표 지점으로 이동
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // 목표 지점 도달 확인
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            MoveToNextWaypoint();
        }
    }

    void MoveToNextWaypoint()
    {
        if (loop)
        {
            // 반복 모드: 0 → 1 → 2 → 3 → 0 → 1 ...
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
        }
        else
        {
            // 왔다갔다 모드: 0 → 1 → 2 → 3 → 2 → 1 → 0 → 1 ...
            if (movingForward)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = waypoints.Length - 2;
                    movingForward = false;
                }
            }
            else
            {
                currentWaypointIndex--;
                if (currentWaypointIndex < 0)
                {
                    currentWaypointIndex = 1;
                    movingForward = true;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage();
            }
        }
    }

    // Scene 뷰에서 경로 시각화
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.red;

        // 포인트들 표시
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);
            }
        }

        // 경로 선 그리기
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }

        // 반복 모드면 마지막에서 첫 번째로 선 연결
        if (loop && waypoints.Length > 0 && waypoints[0] != null && waypoints[waypoints.Length - 1] != null)
        {
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
        }
    }
}
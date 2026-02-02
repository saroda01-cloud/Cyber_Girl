using UnityEngine;

public class CloneFollow2D : MonoBehaviour
{
    [SerializeField] private Rigidbody2D driver;      // 실제 플레이어 RB
    [SerializeField] private Rigidbody2D clone;       // 분신 RB(이 오브젝트)
    [SerializeField] private Vector2 offset;          // 필요하면 오프셋

    private void Reset()
    {
        clone = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        if (clone == null) clone = GetComponent<Rigidbody2D>();

        // Rigidbody2D를 뗄 수 없어도, Kinematic으로 바꾸는 건 가능할 때가 많음
        // (사정상 꼭 Dynamic이어야 한다면 아래 방법 B로)
        clone.bodyType = RigidbodyType2D.Kinematic;
        clone.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void FixedUpdate()
    {
        if (driver == null || clone == null) return;

        // 위치/속도 동기화 (Kinematic은 MovePosition이 안정적)
        Vector2 targetPos = driver.position + offset;
        clone.MovePosition(targetPos);

        // 점프/낙하 느낌 맞추려면 속도도 복사(선택)
        // clone.velocity = driver.velocity; // Kinematic에선 의미가 약해서 필요시만
    }
}

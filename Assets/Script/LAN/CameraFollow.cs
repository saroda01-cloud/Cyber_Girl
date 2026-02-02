using UnityEngine;

public class CameraFollowWithConstraint : MonoBehaviour
{
    [Header("타겟 (자동 찾기)")]
    [SerializeField] private Transform player;

    [Header("배경 제약 (자동 찾기)")]
    private Transform backgroundReference;

    [Header("오프셋")]
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [Header("따라가기 설정")]
    [SerializeField] private bool smoothFollow = true;
    [SerializeField] private float smoothSpeed = 5f;

    private Camera cam;
    private float backgroundHeight;
    private float backgroundYCenter;

    void Start()
    {
        cam = GetComponent<Camera>();

        // 플레이어 자동 찾기
        if (player == null)
        {
            // 방법 1: Tag로 찾기
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                Debug.Log("플레이어 Tag로 자동 할당 완료");
            }
            // 방법 2: 이름으로 찾기
            else
            {
                playerObject = GameObject.Find("Player");
                if (playerObject != null)
                {
                    player = playerObject.transform;
                    Debug.Log("플레이어 이름으로 자동 할당 완료");
                }
            }
        }

        if (player == null)
        {
            Debug.LogError("플레이어를 찾을 수 없습니다! Player Tag를 확인하세요.");
            return;
        }

        // 배경 자동 찾기
        // 방법 1: Tag로 찾기
        GameObject bgObject = GameObject.FindGameObjectWithTag("Background");
        if (bgObject != null)
        {
            backgroundReference = bgObject.transform;
            Debug.Log("배경 Tag로 자동 할당 완료");
        }

        // 방법 2: 이름으로 찾기
        if (backgroundReference == null)
        {
            GameObject bg = GameObject.Find("Background_0");
            if (bg == null) bg = GameObject.Find("Background_1");
            if (bg != null)
            {
                backgroundReference = bg.transform;
                Debug.Log($"배경 이름으로 자동 할당 완료: {backgroundReference.name}");
            }
        }

        // 방법 3: InfiniteBackground 컴포넌트로 찾기
        if (backgroundReference == null)
        {
            InfiniteBackground infiniteBg = FindObjectOfType<InfiniteBackground>();
            if (infiniteBg != null)
            {
                SpriteRenderer[] renderers = infiniteBg.GetComponentsInChildren<SpriteRenderer>();
                if (renderers.Length > 0)
                {
                    backgroundReference = renderers[0].transform;
                }
            }
        }

        if (backgroundReference == null)
        {
            Debug.LogError("배경을 찾을 수 없습니다!");
            return;
        }

        // 배경 정보 계산
        SpriteRenderer sr = backgroundReference.GetComponent<SpriteRenderer>();
        backgroundHeight = sr.bounds.size.y;
        backgroundYCenter = backgroundReference.position.y;

        Debug.Log($"카메라 초기화 완료 - 배경 높이: {backgroundHeight}, 중심 Y: {backgroundYCenter}");
    }

    void LateUpdate()
    {
        // enabled가 false면 실행 안 함
        if (!enabled) return;

        if (backgroundReference == null || player == null) return;

        Vector3 targetPosition = player.position + offset;

        Vector3 newPosition;
        if (smoothFollow)
        {
            newPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
        else
        {
            newPosition = targetPosition;
        }

        float cameraHalfHeight = cam.orthographicSize;
        float backgroundTop = backgroundYCenter + (backgroundHeight / 2f);
        float backgroundBottom = backgroundYCenter - (backgroundHeight / 2f);

        float maxY = backgroundTop - cameraHalfHeight;
        float minY = backgroundBottom + cameraHalfHeight;

        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        transform.position = newPosition;
    }
}
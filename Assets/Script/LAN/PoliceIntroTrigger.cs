using UnityEngine;

public class PoliceIntroTrigger : MonoBehaviour
{
    [Header("Camera Zoom")]
    [SerializeField] private CameraFollowWithConstraint cameraFollow; // 기존 카메라 스크립트
    [SerializeField] private Transform policeTarget; // 경찰(벽) 위치
    [SerializeField] private Transform player; // 플레이어

    [Header("Zoom Settings")]
    [SerializeField] private float zoomInSize = 3f; // 경찰 클로즈업 크기
    [SerializeField] private float zoomInDuration = 2f; // 줌인 시간
    [SerializeField] private float showDuration = 2f; // 경찰 보여주는 시간
    [SerializeField] private float zoomOutDuration = 1f; // 줌아웃 시간

    [Header("Moving Wall")]
    [SerializeField] private MovingWall movingWall; // 밀리는 벽

    private Camera cam;
    private bool hasTriggered = false;

    void Start()
    {
        cam = Camera.main;

        // 자동으로 찾기
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (cameraFollow == null)
        {
            cameraFollow = cam.GetComponent<CameraFollowWithConstraint>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(PoliceIntroSequence());
        }
    }

    System.Collections.IEnumerator PoliceIntroSequence()
    {
        // 1. 카메라 따라가기 비활성화
        if (cameraFollow != null)
        {
            cameraFollow.enabled = false;
        }

        float originalSize = cam.orthographicSize;
        Vector3 originalPos = cam.transform.position;

        // 2. 경찰 위치로 줌인
        yield return StartCoroutine(MoveAndZoomCamera(
            policeTarget.position + new Vector3(0, 0, -10),
            zoomInSize,
            zoomInDuration
        ));

        // 3. 경찰 보여주기
        yield return new WaitForSeconds(showDuration);

        // 4. 플레이어로 복귀
        yield return StartCoroutine(MoveAndZoomCamera(
            player.position + new Vector3(0, 0, -10),
            originalSize,
            zoomOutDuration
        ));

        // 5. 카메라 따라가기 재활성화
        if (cameraFollow != null)
        {
            cameraFollow.enabled = true;
        }

        // 6. 밀리는 벽 시작
        if (movingWall != null)
        {
            movingWall.StartMoving();
        }
    }

    System.Collections.IEnumerator MoveAndZoomCamera(Vector3 targetPos, float targetSize, float duration)
    {
        Vector3 startPos = cam.transform.position;
        float startSize = cam.orthographicSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Smooth easing
            t = t * t * (3f - 2f * t);

            cam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            yield return null;
        }

        cam.transform.position = targetPos;
        cam.orthographicSize = targetSize;
    }
}
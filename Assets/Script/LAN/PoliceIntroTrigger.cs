using UnityEngine;

public class PoliceIntroTrigger : MonoBehaviour
{
    [Header("수동 설정 필요")]
    [SerializeField] private Transform policeTarget;
    [SerializeField] private MovingWall movingWall;

    private CameraFollowWithConstraint cameraFollow;
    private Transform player;
    private PlayerController playerController; // 플레이어 컨트롤러 참조 추가

    [Header("Zoom Settings")]
    [SerializeField] private float zoomInSize = 4f;
    [SerializeField] private float zoomInDuration = 2f;
    [SerializeField] private float showDuration = 2f;
    [SerializeField] private float zoomOutDuration = 1f;

    private Camera cam;
    private bool hasTriggered = false;

    void Start()
    {
        cam = Camera.main;

        // 자동으로 찾기
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerController = playerObj.GetComponent<PlayerController>(); // PlayerController 가져오기
            }
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
        // 1. 플레이어 이동 막기
        if (playerController != null)
        {
            playerController.SetMovementEnabled(false);
        }

        // 2. 카메라 따라가기 비활성화
        if (cameraFollow != null)
        {
            cameraFollow.enabled = false;
        }

        float originalSize = cam.orthographicSize;
        Vector3 originalPos = cam.transform.position;

        // 3. 경찰 위치로 줌인
        yield return StartCoroutine(MoveAndZoomCamera(
            policeTarget.position + new Vector3(0, 0, -10),
            zoomInSize,
            zoomInDuration
        ));

        // 4. 경찰 보여주기
        yield return new WaitForSeconds(showDuration);

        // 5. 플레이어로 복귀
        yield return StartCoroutine(MoveAndZoomCamera(
            player.position + new Vector3(0, 0, -10),
            originalSize,
            zoomOutDuration
        ));

        // 6. 카메라 따라가기 재활성화
        if (cameraFollow != null)
        {
            cameraFollow.enabled = true;
        }

        // 7. 플레이어 이동 재활성화
        if (playerController != null)
        {
            playerController.SetMovementEnabled(true);
        }

        // 8. 밀리는 벽 시작
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
            t = t * t * (3f - 2f * t);

            cam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            yield return null;
        }

        cam.transform.position = targetPos;
        cam.orthographicSize = targetSize;
    }
}
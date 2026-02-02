using System.Collections;
using UnityEngine;

public class DayDirector : MonoBehaviour
{
    [Header("필수 연결")]
    [SerializeField] private DialogTest dialogTest;
    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Rigidbody2D NPCRigidbody;
    [SerializeField] private Animator NPCAnimator;
    [SerializeField] private SpriteRenderer NPCSprite;


   [Header("Day2 카메라 parent 대상(= gameobject의 자식 Transform)")]
    // 예: NPC 아래에 빈 오브젝트 CameraAnchor 만들어두고 그 Transform 넣기
    [SerializeField] private Transform day2CameraParent;

    [Header("Day2 플레이어 자동 이동")]
    [SerializeField] private float playerMoveSpeedX = 3f;  // 왼쪽 이동 속도
    [SerializeField] private float playerMoveDuration = 2f;   // 몇 초 이동할지

    [Header("Day2 카메라 로컬 이동(오른쪽으로 조금)")]
    [SerializeField] private Vector3 cameraLocalMove = new Vector3(0f, 0f, 0f);
    [SerializeField] private float cameraMoveTime = 0.25f; // 부드럽게 이동 시간(원하면 0으로 즉시)

    [Header("Day2 NPC 자동 이동")]
    [SerializeField] private float NPCMoveSpeedX = -3f;  // 오른쪽 이동 속도
    [SerializeField] private float NPCMoveDuration = 2f;   // 몇 초 이동할지

    private bool day2Played = false;

    private void Awake()
    {
        if (dialogTest != null)
            dialogTest.OnDayDialogFinished += HandleDayDialogFinished;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void OnDestroy()
    {
        if (dialogTest != null)
            dialogTest.OnDayDialogFinished -= HandleDayDialogFinished;
    }

    private void HandleDayDialogFinished(int finishedDay)
    {
        if (finishedDay == 4)
        {
            // Day 4는 바로 Map5로
            SceneTransitionManager.Instance.currentDay = 5;
            SceneTransitionManager.Instance.LoadScene("Map5");
        }
        else if (finishedDay == 2 && !day2Played)
        {
            // Day 2는 특별한 연출 후 First로
            day2Played = true;
            StartCoroutine(PlayDay2Sequence());
        }
        else
        {
            // Day 1, 3은 바로 First로
            SceneTransitionManager.Instance.ContinueFromVenue();
        }
    }
    private IEnumerator PlayDay2Sequence()
    {
        Debug.Log("[DayDirector] PlayDay2Sequence 시작!");

        // 1) 플레이어 2초 이동
        yield return StartCoroutine(MovePlayerForSeconds(playerMoveDuration));
        Debug.Log("[DayDirector] 플레이어 이동 완료");

        // 2) 플레이어 이동 끝난 뒤 "다른 기능" 실행
        yield return StartCoroutine(AfterPlayerMove());
        Debug.Log("[DayDirector] AfterPlayerMove 완료");

        // 3) 씬 전환
        Debug.Log("[DayDirector] First로 이동 시작!");
        SceneTransitionManager.Instance.ContinueFromVenue();
    }
    private IEnumerator MovePlayerForSeconds(float seconds)
    {
        if (playerSprite != null)
            playerSprite.flipX = (playerMoveSpeedX < 0f);
        float t = 0f;

        while (t < seconds)
        {
            Vector2 nextPos = playerRigidbody.position + Vector2.right * playerMoveSpeedX * Time.fixedDeltaTime;
            playerRigidbody.MovePosition(nextPos);

            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        playerRigidbody.linearVelocity = Vector2.zero;
        playerRigidbody.angularVelocity = 0f;
    }

    private IEnumerator AfterPlayerMove()
    {
        // 카메라를 "특정 gameobject의 자식"으로 넣기
        if (mainCamera == null || day2CameraParent == null)
        {
            Debug.LogWarning("[DayDirector] mainCamera 또는 day2CameraParent가 연결되지 않아 Day2 카메라 연출이 스킵됩니다.");
            yield break;
        }

        Transform camTr = mainCamera.transform;

        // parent 변경 + 위치 세팅
        camTr.SetParent(day2CameraParent, true);
        camTr.localPosition = new Vector3(0, 0, -10f);

        // 카메라 로컬 이동
        if (cameraMoveTime <= 0f)
        {
            camTr.localPosition += cameraLocalMove;
        }
        else
        {
            Vector3 start = camTr.localPosition;
            Vector3 end = start + cameraLocalMove;

            float ct = 0f;
            while (ct < cameraMoveTime)
            {
                ct += Time.deltaTime;
                float a = Mathf.Clamp01(ct / cameraMoveTime);
                camTr.localPosition = Vector3.Lerp(start, end, a);
                yield return null;
            }
            camTr.localPosition = end;
        }

        // NPC 이동
        if (NPCRigidbody != null)
        {
            // NPC 방향(실제 이동 방향 기준)
            if (NPCSprite != null)
                NPCSprite.flipX = (NPCMoveSpeedX < 0f);

            // 걷기 애니메이션 ON
            if (NPCAnimator != null)
                NPCAnimator.SetBool("isWalk", true);

            float t2 = 0f;
            while (t2 < NPCMoveDuration)
            {
                Vector2 nextPos = NPCRigidbody.position + Vector2.right * NPCMoveSpeedX * Time.fixedDeltaTime;
                NPCRigidbody.MovePosition(nextPos);

                t2 += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            // 걷기 애니메이션 OFF
            if (NPCAnimator != null)
                NPCAnimator.SetBool("isWalk", false);

            NPCRigidbody.linearVelocity = Vector2.zero;
            NPCRigidbody.angularVelocity = 0f;
        }
    }

}



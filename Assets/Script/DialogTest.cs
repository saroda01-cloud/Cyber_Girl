using System.Collections;
using UnityEngine;

public class DialogTest : MonoBehaviour
{
    [Header("거리 조건")]
    [SerializeField] private Transform player;              // 플레이어 Transform (없으면 Tag로 자동 탐색)
    [SerializeField] private Transform talkTarget;          // 대화 시작 거리 측정 대상 (비우면 이 오브젝트)
    [SerializeField] private float startDistance = 5f;      // 이 거리 이하가 되면 대화 시작
    [SerializeField] private bool use2DDistance = true;     // 2D면 true 추천 (Z 무시)

    [Header("Day 설정")]
    [SerializeField] private int currentDay = 1; // 1부터 시작한다고 가정

    [Header("Day별 실행할 DialogSystem (index 0 = Day1)")]
    [SerializeField] private DialogSystem[] dialogsByDay;

    private bool hasStarted = false;
    public bool IsInTalkRange { get; private set; }



    private void Update()
    {
        float distance = GetDistance(player.position, talkTarget.position);

        // 대화 거리 안으로 들어온 순간
        if (distance <= startDistance)
        {
            IsInTalkRange = true;
        }
        else
        {
            IsInTalkRange = false;
        }
    }

    private IEnumerator Start()
    {
        // talkTarget이 비어있으면 이 오브젝트를 기준으로 거리 체크
        if (talkTarget == null) talkTarget = transform;

        // player가 비어있으면 Tag가 Player인 오브젝트를 찾아봄
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player == null)
        {
            Debug.LogError("[DialogTest] player가 지정되지 않았고, Tag=Player도 찾지 못했습니다. 인스펙터에 player를 넣어주세요.");
            yield break;
        }

        if (dialogsByDay == null || dialogsByDay.Length == 0)
        {
            Debug.LogError("[DialogTest] dialogsByDay가 비어있습니다. 인스펙터에서 Day별 DialogSystem을 넣어주세요.");
            yield break;
        }

        // 거리 조건 만족할 때까지 대기 (이 전에는 dialog 출력/시작 안 함)
        yield return new WaitUntil(() => GetDistance(player.position, talkTarget.position) <= startDistance);

        // (혹시 Start가 여러 번 호출되는 구조로 바뀌어도 1회만 시작되게)
        if (hasStarted) yield break;
        hasStarted = true;

        int index = currentDay - 1; // Day1 -> 0
        if (index < 0 || index >= dialogsByDay.Length)
        {
            Debug.LogError($"[DialogTest] currentDay={currentDay} 가 dialogsByDay 범위를 벗어났습니다. (0~{dialogsByDay.Length - 1})");
            yield break;
        }

        DialogSystem target = dialogsByDay[index];
        if (target == null)
        {
            Debug.LogError($"[DialogTest] dialogsByDay[{index}] 가 비어있습니다. (Day {currentDay})");
            yield break;
        }

        // Day1~2는 글자 대신 스프라이트 모드
        target.SetUseSpriteDialogue(currentDay <= 2);

        // 항상 처음부터 시작하도록 리셋
        target.ResetDialog();

        // Day에 해당하는 대사 분기 시작 (Space로 넘기는 기존 방식 그대로)
        yield return new WaitUntil(() => target.UpdateDialog());

        currentDay++;
    }

    private float GetDistance(Vector3 a, Vector3 b)
    {
        if (use2DDistance)
        {
            // 2D: Z 무시하고 거리 계산
            return Vector2.Distance(new Vector2(a.x, a.y), new Vector2(b.x, b.y));
        }
        return Vector3.Distance(a, b);
    }
}




using System.Collections;
using UnityEngine;

public class DialogTest : MonoBehaviour
{
    [Header("Day 설정")]
    [SerializeField] private int currentDay = 1; // 1부터 시작한다고 가정

    [Header("Day별 실행할 DialogSystem (index 0 = Day1)")]
    [SerializeField] private DialogSystem[] dialogsByDay;

    private IEnumerator Start()
    {
        if (dialogsByDay == null || dialogsByDay.Length == 0)
        {
            Debug.LogError("[DialogTest] dialogsByDay가 비어있습니다. 인스펙터에서 Day별 DialogSystem을 넣어주세요.");
            yield break;
        }

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

        // Day에 해당하는 대사 분기 시작
        yield return new WaitUntil(() => target.UpdateDialog());

        currentDay++;
    }
}



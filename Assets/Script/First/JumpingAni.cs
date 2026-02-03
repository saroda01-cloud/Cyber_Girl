using UnityEngine;
using System.Collections;

public class JumpAnimation : MonoBehaviour
{
    [Header("점프 설정")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpDistance = 3f;
    [SerializeField] private float jumpDuration = 1f;

    [Header("시작 시 점프")]
    [SerializeField] private int initialJumpCount = 3; // 처음 점프 횟수
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private bool startOnAwake = true;

    [Header("시퀀스 후 점프")]
    [SerializeField] private int sequenceJumpCount = 1; // 스프라이트 시퀀스 후 점프 횟수
    [SerializeField] private Vector3 sequenceStartPosition;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = startPosition;

        if (startOnAwake)
        {
            StartInitialJump();
        }
    }

    // 처음 점프 (3번)
    public void StartInitialJump()
    {
        transform.position = startPosition;
        StartCoroutine(JumpSequence(startPosition, initialJumpCount));
    }

    // 스프라이트 시퀀스 후 점프 (1번)
    public void StartSequenceJump()
    {
        transform.position = sequenceStartPosition;
        StartCoroutine(JumpSequence(sequenceStartPosition, sequenceJumpCount));
    }

    // 총 점프 시간 계산
    public float GetJumpTime(int count)
    {
        return jumpDuration * count;
    }

    private IEnumerator JumpSequence(Vector3 start, int count)
    {
        Vector3 currentPos = start;

        for (int i = 0; i < count; i++)
        {
            Vector3 jumpStart = currentPos;
            Vector3 jumpEnd = currentPos + new Vector3(jumpDistance, 0f, 0f);

            yield return StartCoroutine(PerformJump(jumpStart, jumpEnd));

            currentPos = jumpEnd;
        }

        Debug.Log($"점프 애니메이션 완료! ({count}번)");
    }

    private IEnumerator PerformJump(Vector3 startPos, Vector3 endPos)
    {
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;

            float x = Mathf.Lerp(startPos.x, endPos.x, t);
            float y = startPos.y + (-4f * jumpHeight * Mathf.Pow(t - 0.5f, 2f) + jumpHeight);

            transform.position = new Vector3(x, y, startPos.z);

            yield return null;
        }

        transform.position = endPos;
    }
}
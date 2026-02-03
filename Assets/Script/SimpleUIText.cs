using UnityEngine;
using TMPro;
using System.Collections;

public class SimpleMessageTrigger : MonoBehaviour
{
    [Header("메시지 설정")]
    [TextArea(2, 5)]
    [SerializeField] private string message = "메시지를 입력하세요";

    [Header("폰트 설정")]
    [SerializeField] private TMP_FontAsset customFont;
    [SerializeField] private float fontSize = 36f;
    [SerializeField] private Color textColor = Color.white;

    [Header("위치 설정")]
    [SerializeField] private Vector2 anchorPosition = new Vector2(0.5f, 0.8f); // 0~1 (0.5, 0.8 = 중앙 상단)
    [SerializeField] private Vector2 size = new Vector2(600, 100);

    [Header("표시 시간")]
    [SerializeField] private float displayDuration = 3f;

    [Header("한 번만 트리거")]
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered = false;
    private static GameObject currentMessageUI;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnce && hasTriggered) return;

            hasTriggered = true;
            ShowMessage();
        }
    }

    void ShowMessage()
    {
        // 이전 메시지 제거
        if (currentMessageUI != null)
        {
            Destroy(currentMessageUI);
        }

        // Canvas 찾기
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas를 찾을 수 없습니다!");
            return;
        }

        // UI 생성
        GameObject messageObj = new GameObject("MessagePopup");
        messageObj.transform.SetParent(canvas.transform, false);

        // RectTransform 설정
        RectTransform rectTransform = messageObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorPosition;
        rectTransform.anchorMax = anchorPosition;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = size;

        // TextMeshPro 추가
        TextMeshProUGUI tmp = messageObj.AddComponent<TextMeshProUGUI>();
        tmp.text = message;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = textColor;

        // 커스텀 폰트 적용
        if (customFont != null)
        {
            tmp.font = customFont;
        }

        // CanvasGroup으로 페이드 효과
        CanvasGroup canvasGroup = messageObj.AddComponent<CanvasGroup>();

        currentMessageUI = messageObj;

        // 자동 제거
        StartCoroutine(FadeOutAndDestroy(messageObj, canvasGroup));
    }

    IEnumerator FadeOutAndDestroy(GameObject obj, CanvasGroup cg)
    {
        // 표시 시간 대기
        yield return new WaitForSeconds(displayDuration);

        // 페이드 아웃
        float fadeTime = 0.5f;
        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            cg.alpha = 1f - (elapsed / fadeTime);
            yield return null;
        }

        // 삭제
        if (obj != null) Destroy(obj);
        if (currentMessageUI == obj) currentMessageUI = null;
    }
}
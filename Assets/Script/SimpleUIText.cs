using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SimpleMessageTrigger : MonoBehaviour
{
    [Header("표시 모드")]
    [SerializeField] private bool useSprite = false; // true면 스프라이트, false면 텍스트

    [Header("텍스트 설정")]
    [TextArea(2, 5)]
    [SerializeField] private string message = "메시지를 입력하세요";
    [SerializeField] private TMP_FontAsset customFont;
    [SerializeField] private float fontSize = 36f;
    [SerializeField] private Color textColor = Color.white;

    [Header("스프라이트 설정")]
    [SerializeField] private Sprite messageSprite;

    [Header("위치 설정")]
    [SerializeField] private Vector2 anchorPosition = new Vector2(0.5f, 0.8f);
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

        if (useSprite && messageSprite != null)
        {
            // 스프라이트 모드
            Image img = messageObj.AddComponent<Image>();
            img.sprite = messageSprite;
            img.preserveAspect = true; // 비율 유지
        }
        else
        {
            // 텍스트 모드
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

        // 오브젝트가 이미 삭제되었는지 확인
        if (obj == null || cg == null)
        {
            yield break;
        }

        // 페이드 아웃
        float fadeTime = 0.5f;
        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            // 페이드 중에도 오브젝트 존재 확인
            if (obj == null || cg == null)
            {
                yield break;
            }

            elapsed += Time.deltaTime;
            cg.alpha = 1f - (elapsed / fadeTime);
            yield return null;
        }

        // 삭제 전 마지막 확인
        if (obj != null)
        {
            Destroy(obj);
        }

        if (currentMessageUI == obj)
        {
            currentMessageUI = null;
        }
    }
}
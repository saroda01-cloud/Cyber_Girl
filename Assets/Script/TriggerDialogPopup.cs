using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TriggerDialogPopup : MonoBehaviour
{
    [Header("UI Prefab (TestDialog 프리펩 권장)")]
    [SerializeField] private GameObject uiPrefab;

    [Header("UI 부모(비우면 Canvas 자동 탐색)")]
    [SerializeField] private Transform uiParent;

    [Header("플레이어 태그")]
    [SerializeField] private string playerTag = "Player";

    [Header("트리거 1회만")]
    [SerializeField] private bool triggerOnce = true;

    [Header("표시 내용(텍스트 / 스프라이트)")]
    [TextArea(2, 6)]
    [SerializeField] private string messageText;

    [SerializeField] private Sprite messageSprite;

    [Tooltip("Sprite가 있으면 텍스트 대신 그림을 표시")]
    [SerializeField] private bool preferSpriteIfAssigned = true;

    [Header("표시 시간 / 페이드아웃")]
    [SerializeField] private float showDuration = 5f;
    [SerializeField] private float fadeOutDuration = 0.75f;

    // 전역: 동시에 하나만
    private static GameObject currentUIRoot;
    private static Coroutine currentCoroutine;
    private static MonoBehaviour coroutineOwner;

    private bool hasTriggered = false;

    private void Reset()
    {
        var col2D = GetComponent<Collider2D>();
        if (col2D != null) col2D.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnce && hasTriggered) return;
        if (!other.CompareTag(playerTag)) return;

        hasTriggered = true;
        ShowPopup();
    }

    private void ShowPopup()
    {
        if (uiPrefab == null)
        {
            Debug.LogWarning("[TriggerDialogPopup] uiPrefab이 비어있음.");
            return;
        }

        StopAndClearCurrentUI();

        Transform parent = uiParent;
        if (parent == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null) parent = canvas.transform;
        }

        if (parent == null)
        {
            Debug.LogWarning("[TriggerDialogPopup] Canvas(uiParent)를 찾지 못했음.");
            return;
        }

        // 1) 일단 프리펩 생성
        GameObject instance = Instantiate(uiPrefab, parent);

        // 2) "Canvas 바로 아래 최상위 컨테이너"를 루트로 잡기
        //    (프리펩을 Textdialog 같은 자식으로 넣었어도 TestDialog까지 끌어올려 삭제 가능)
        GameObject uiRoot = GetTopMostUnderCanvas(instance.transform);

        // 3) 참조 찾기 (루트 기준으로 찾기)
        TMP_Text text = FindTMP(uiRoot.transform, "Textdialog");
        Image spriteImage = FindImage(uiRoot.transform, "DialogSprite");

        // 4) CanvasGroup은 반드시 "루트"에 붙여서 전체가 함께 페이드되게
        CanvasGroup cg = uiRoot.GetComponent<CanvasGroup>();
        if (cg == null) cg = uiRoot.AddComponent<CanvasGroup>();

        cg.alpha = 1f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        // 5) 내용 적용
        bool useSprite = preferSpriteIfAssigned && messageSprite != null;

        if (useSprite)
        {
            if (spriteImage != null)
            {
                spriteImage.sprite = messageSprite;
                spriteImage.gameObject.SetActive(true);
            }
            if (text != null) text.gameObject.SetActive(false);
        }
        else
        {
            if (text != null)
            {
                text.text = messageText;
                text.gameObject.SetActive(true);
            }
            if (spriteImage != null) spriteImage.gameObject.SetActive(false);
        }

        // 6) 전역 등록 (루트 기준)
        currentUIRoot = uiRoot;
        coroutineOwner = this;
        currentCoroutine = StartCoroutine(ShowThenFadeOut(uiRoot, cg));
    }

    private IEnumerator ShowThenFadeOut(GameObject uiRoot, CanvasGroup cg)
    {
        yield return new WaitForSeconds(showDuration);

        float t = 0f;
        float start = cg.alpha;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, 0f, t / fadeOutDuration);
            yield return null;
        }

        cg.alpha = 0f;

        if (uiRoot != null) Destroy(uiRoot);

        if (currentUIRoot == uiRoot)
        {
            currentUIRoot = null;
            currentCoroutine = null;
            coroutineOwner = null;
        }
    }

    private static void StopAndClearCurrentUI()
    {
        if (currentCoroutine != null && coroutineOwner != null)
        {
            coroutineOwner.StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        if (currentUIRoot != null)
        {
            Destroy(currentUIRoot);
            currentUIRoot = null;
        }

        coroutineOwner = null;
    }

    // Canvas 바로 아래에 붙어있는 최상위(UI틀) 오브젝트 반환
    private static GameObject GetTopMostUnderCanvas(Transform t)
    {
        Transform cur = t;
        while (cur.parent != null && cur.parent.GetComponent<Canvas>() == null)
        {
            cur = cur.parent;
        }
        return cur.gameObject;
    }

    private static TMP_Text FindTMP(Transform root, string childName)
    {
        Transform t = FindDeepChild(root, childName);
        return t != null ? t.GetComponent<TMP_Text>() : null;
    }

    private static Image FindImage(Transform root, string childName)
    {
        Transform t = FindDeepChild(root, childName);
        return t != null ? t.GetComponent<Image>() : null;
    }

    private static Transform FindDeepChild(Transform parent, string name)
    {
        if (parent.name == name) return parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Transform result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }
}






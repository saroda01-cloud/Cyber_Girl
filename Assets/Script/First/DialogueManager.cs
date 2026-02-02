using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject spacebarIcon;

    [Header("타이핑 설정")]
    [SerializeField] private float typingSpeed = 0.05f; // 글자 재생 속도 (인스펙터 조절)

    [Header("스페이스바 블링크")]
    [SerializeField] private float blinkSpeed = 0.5f; // 블링크 속도

    private string[] dialogues;
    private int currentDialogueIndex = 0;
    private bool isTyping = false;
    private Coroutine blinkCoroutine;

    public bool IsDialogueFinished => currentDialogueIndex >= dialogues.Length;
    public System.Action OnDialogueComplete;

    void Update()
    {
        // 스페이스바 또는 마우스 클릭으로 넘기기
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !isTyping && dialogues != null)
        {
            NextDialogue();
        }
    }

    public void SetDialogues(string[] newDialogues)
    {
        dialogues = newDialogues;
    }

    public void StartDialogue()
    {
        if (dialogues == null || dialogues.Length == 0) return;

        currentDialogueIndex = 0;
        ShowDialogue();
    }

    private void NextDialogue()
    {
        if (currentDialogueIndex < dialogues.Length)
        {
            ShowDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    private void ShowDialogue()
    {
        if (currentDialogueIndex < dialogues.Length)
        {
            StartCoroutine(TypeText(dialogues[currentDialogueIndex]));
            currentDialogueIndex++;
        }
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;

        // 타이핑 중에는 스페이스바 아이콘 숨기기
        if (spacebarIcon != null)
        {
            spacebarIcon.SetActive(false);
            StopBlinking(); // 블링크 중지
        }

        dialogueText.text = "";

        // 조절 가능한 타이핑 속도
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        // 타이핑 완료 후 스페이스바 아이콘 표시 및 블링크 시작
        if (spacebarIcon != null)
        {
            spacebarIcon.SetActive(true);
            StartBlinking();
        }
    }

    private void StartBlinking()
    {
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkSpacebarIcon());
    }

    private void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
    }

    private IEnumerator BlinkSpacebarIcon()
    {
        while (spacebarIcon != null && spacebarIcon.activeInHierarchy)
        {
            // 페이드 아웃
            yield return StartCoroutine(FadeIcon(1f, 0.3f));
            // 페이드 인
            yield return StartCoroutine(FadeIcon(0.3f, 1f));
        }
    }

    private IEnumerator FadeIcon(float fromAlpha, float toAlpha)
    {
        Image iconImage = spacebarIcon.GetComponent<Image>();
        if (iconImage == null) yield break;

        float elapsedTime = 0f;
        Color originalColor = iconImage.color;

        while (elapsedTime < blinkSpeed)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / blinkSpeed);
            iconImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        iconImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, toAlpha);
    }

    private void EndDialogue()
    {
        StopBlinking(); // 대화 종료 시 블링크 중지
        OnDialogueComplete?.Invoke();
    }
}
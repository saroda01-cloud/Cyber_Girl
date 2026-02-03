using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [SerializeField] private Speaker[] speakers;
    [SerializeField] private DialogData[] dialogs;
    [SerializeField] private bool isAutoStart = true;

    private bool isFirst = true;
    private int currentDialogIndex = -1;
    private int currentSpeakerIndex = 0;
    private float typingSpeed = 0.05f;
    private bool isTypingEffect = false;

    [Header("Day1~2: 글자 대신 Sprite 대사")]
    [SerializeField] private bool useSpriteDialogue = false;

    // 현재 켜져있는 씬 스프라이트 추적
    private SpriteRenderer currentActiveSceneSprite = null;

    // 코루틴 핸들
    private Coroutine typingCoroutine;

    private const string AnimatorParamExpression = "exPression";

    private void Awake()
    {
        Setup();
    }

    private void Setup()
    {
        for (int i = 0; i < speakers.Length; ++i)
        {
            SetActiveObjects(speakers[i], false);
            speakers[i].spriteRenderer.gameObject.SetActive(true);
        }

        SetSceneSpriteForLine(-1);
    }

    public bool UpdateDialog()
    {
        if (isFirst)
        {
            Setup();
            if (isAutoStart) SetNextDialog();
            isFirst = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //  타이핑 중 → 즉시 전체 출력
            if (isTypingEffect)
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                }

                isTypingEffect = false;

                speakers[currentSpeakerIndex].textDialogue.text =
                    dialogs[currentDialogIndex].dialogue;

                speakers[currentSpeakerIndex].objectArrow.SetActive(true);

                return false; // 입력 소모
            }
            // 타이핑 완료 상태 → 다음 대사
            else if (speakers[currentSpeakerIndex].objectArrow.activeSelf)
            {
                if (dialogs.Length > currentDialogIndex + 1)
                {
                    SetNextDialog();
                }
                else
                {
                    // 대화 종료
                    for (int i = 0; i < speakers.Length; ++i)
                    {
                        SetActiveObjects(speakers[i], false);
                        speakers[i].spriteRenderer.gameObject.SetActive(true);
                    }

                    SetSceneSpriteForLine(-1);
                    return true;
                }
            }
        }

        return false;
    }

    private void ApplyExpressionForLine(int lineIndex)
    {
        if (lineIndex < 0 || dialogs == null || lineIndex >= dialogs.Length) return;

        int speakerIndex = dialogs[lineIndex].speakerIndex;
        if (speakerIndex < 0 || speakerIndex >= speakers.Length) return;

        Animator anim = speakers[speakerIndex].animator;
        if (anim == null) return;

        anim.SetInteger(AnimatorParamExpression, (int)dialogs[lineIndex].expression);
    }

    private void SetNextDialog()
    {
        // 이전 화자 UI 끄기
        SetActiveObjects(speakers[currentSpeakerIndex], false);

        currentDialogIndex++;

        // 씬 스프라이트 처리
        SetSceneSpriteForLine(currentDialogIndex);

        // 현재 화자 설정
        currentSpeakerIndex = dialogs[currentDialogIndex].speakerIndex;

        // 표정 적용
        ApplyExpressionForLine(currentDialogIndex);

        // UI 켜기
        SetActiveObjects(speakers[currentSpeakerIndex], true);

        // Sprite 대사 모드
        if (useSpriteDialogue && dialogs[currentDialogIndex].dialogueSprite != null)
        {
            speakers[currentSpeakerIndex].imageDialogueSprite.sprite =
                dialogs[currentDialogIndex].dialogueSprite;

            speakers[currentSpeakerIndex].objectArrow.SetActive(true);
        }
        else
        {
            typingCoroutine = StartCoroutine(OnTypingText());
        }
    }

    // 씬 스프라이트 제어
    private void SetSceneSpriteForLine(int lineIndex)
    {
        if (currentActiveSceneSprite != null)
        {
            currentActiveSceneSprite.gameObject.SetActive(false);
            currentActiveSceneSprite = null;
        }

        if (lineIndex < 0 || dialogs == null || lineIndex >= dialogs.Length)
            return;

        var sr = dialogs[lineIndex].sceneSpriteRenderer;
        if (sr != null)
        {
            sr.gameObject.SetActive(true);
            currentActiveSceneSprite = sr;
        }
    }

    private void SetActiveObjects(Speaker speaker, bool visible)
    {
        speaker.imageDialog.gameObject.SetActive(visible);

        bool hasSprite =
            useSpriteDialogue &&
            dialogs != null &&
            currentDialogIndex >= 0 &&
            currentDialogIndex < dialogs.Length &&
            dialogs[currentDialogIndex].dialogueSprite != null;

        if (speaker.imageDialogueSprite != null)
            speaker.imageDialogueSprite.gameObject.SetActive(visible && hasSprite);

        speaker.textDialogue.gameObject.SetActive(visible && !hasSprite);

        speaker.objectArrow.SetActive(false);
    }

    private IEnumerator OnTypingText()
    {
        int index = 0;
        isTypingEffect = true;

        while (index < dialogs[currentDialogIndex].dialogue.Length)
        {
            speakers[currentSpeakerIndex].textDialogue.text =
                dialogs[currentDialogIndex].dialogue.Substring(0, index + 1);

            index++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;
        speakers[currentSpeakerIndex].objectArrow.SetActive(true);
        typingCoroutine = null;
    }

    public void ResetDialog()
    {
        isFirst = true;
        currentDialogIndex = -1;
        currentSpeakerIndex = 0;
        isTypingEffect = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        Setup();
    }

    public void SetDialogs(DialogData[] newDialogs)
    {
        dialogs = newDialogs;
        ResetDialog();
    }

    public void SetUseSpriteDialogue(bool value)
    {
        useSpriteDialogue = value;
    }

    public enum FaceExpression
    {
        Idle = 0,
        Surprise = 1,
        Happy = 2,
        Sick = 3,
        Question = 4
    }
}

[System.Serializable]
public struct Speaker
{
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Image imageDialog;
    public TextMeshProUGUI textDialogue;
    public Image imageDialogueSprite;
    public GameObject objectArrow;
}

[System.Serializable]
public struct DialogData
{
    public int speakerIndex;
    public string name;

    [TextArea(3, 5)]
    public string dialogue;

    public Sprite dialogueSprite;

    [Header("표정(Animator)")]
    public DialogSystem.FaceExpression expression;

    [Header("씬 스프라이트(선택)")]
    public SpriteRenderer sceneSpriteRenderer;
    public bool sceneSpriteVisibleOnlyThisLine;
}

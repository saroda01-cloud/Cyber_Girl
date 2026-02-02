using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DialogSystem;

public class DialogSystem : MonoBehaviour
{
    [SerializeField] private Speaker[] speakers;
    [SerializeField] private DialogData[] dialogs;
    [SerializeField] private bool isAutoStart = true;

    private bool isFirst = true;
    private int currentDialogIndex = -1;
    private int currentSpeakerIndex = 0;
    private float typingSpeed = 0.1f;
    private bool isTypingEffect = false;

    [Header("Day1~2: 글자 대신 Sprite 대사")]
    [SerializeField] private bool useSpriteDialogue = false;

    // 현재 켜져있는 "씬 스프라이트"를 추적해서 다음 줄에서 끄기 위함
    private SpriteRenderer currentActiveSceneSprite = null;

    private void Awake()
    {
        Setup();
    }

    private void Setup()
    {
        // 모든 대화 관련 게임오브젝트 비활성화
        for (int i = 0; i < speakers.Length; ++i)
        {
            SetActiveObjects(speakers[i], false);
            speakers[i].spriteRenderer.gameObject.SetActive(true);
        }

        // 혹시 이전에 켜진 씬 스프라이트가 있으면 끄기
        SetSceneSpriteForLine(-1);
    }

    public bool UpdateDialog()
    {
        if (isFirst == true)
        {
            Setup();
            if (isAutoStart) SetNextDialog();
            isFirst = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTypingEffect == true)
            {
                isTypingEffect = false;

                StopCoroutine("OnTypingText");
                speakers[currentSpeakerIndex].textDialogue.text = dialogs[currentDialogIndex].dialogue;
                speakers[currentSpeakerIndex].objectArrow.SetActive(true);

                return false;
            }

            if (dialogs.Length > currentDialogIndex + 1)
            {
                SetNextDialog();
            }
            else
            {
                for (int i = 0; i < speakers.Length; ++i)
                {
                    SetActiveObjects(speakers[i], false);
                    speakers[i].spriteRenderer.gameObject.SetActive(true);
                }

                // 대화 끝나면 켜져있던 씬 스프라이트도 끄기
                SetSceneSpriteForLine(-1);

                return true;
            }
        }

        return false;
    }

    private const string AnimatorParamExpression = "exPression";
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

        // 이전 줄의 씬 스프라이트 끄기 + (새 줄) 씬 스프라이트 켜기
        // (currentDialogIndex가 아직 증가 전이므로, 아래에서 증가 후 처리)
        // 다음 대사로
        currentDialogIndex++;

        // 현재 줄 기준으로 씬 스프라이트 반영
        SetSceneSpriteForLine(currentDialogIndex);

        // 현재 화자 설정
        currentSpeakerIndex = dialogs[currentDialogIndex].speakerIndex;

        // 표정 적용    
        ApplyExpressionForLine(currentDialogIndex);

        // 현재 화자 UI 켜기
        SetActiveObjects(speakers[currentSpeakerIndex], true);

        // Day1~2 + Sprite가 있는 경우
        if (useSpriteDialogue && dialogs[currentDialogIndex].dialogueSprite != null)
        {
            speakers[currentSpeakerIndex].imageDialogueSprite.sprite =
                dialogs[currentDialogIndex].dialogueSprite;

            speakers[currentSpeakerIndex].objectArrow.SetActive(true);
        }
        else
        {
            StartCoroutine(OnTypingText());
        }
    }

    //  이 함수가 "씬에 있는 스프라이트"를 대사별로 켜고/끄는 역할
    private void SetSceneSpriteForLine(int lineIndex)
    {
        // 1) 이전에 켜져 있던 씬 스프라이트 끄기
        if (currentActiveSceneSprite != null)
        {
            currentActiveSceneSprite.gameObject.SetActive(false);
            currentActiveSceneSprite = null;
        }

        // lineIndex가 유효하지 않으면 종료
        if (lineIndex < 0 || dialogs == null || lineIndex >= dialogs.Length)
            return;

        // 2) 현재 줄에 씬 스프라이트가 지정되어 있다면 켜기
        var sr = dialogs[lineIndex].sceneSpriteRenderer;
        if (sr != null)
        {
            sr.gameObject.SetActive(true);
            currentActiveSceneSprite = sr;

            // 옵션: "이 줄에서만" 보여주기 (기본 true로 쓸 거면 이 bool은 굳이 없어도 됨)
            // 지금 구현은 항상 '다음 줄로 넘어가면 끄기'라서, 사실상 "이 줄에서만"과 동일하게 동작함.
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
    }

    public void ResetDialog()
    {
        isFirst = true;
        currentDialogIndex = -1;
        currentSpeakerIndex = 0;
        isTypingEffect = false;

        StopAllCoroutines();
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
    public int speakerIndex;                    // 화자 index
    public string name;                         // 캐릭터 이름

    [TextArea(3, 5)]
    public string dialogue;                     // 대사(텍스트)

    public Sprite dialogueSprite;               // Day1~2에서 사용할 대사 스프라이트 (없으면 텍스트로 폴백)

    [Header("표정(Animator)")]
    public FaceExpression expression;

    [Header("씬 스프라이트(선택)")]
    public SpriteRenderer sceneSpriteRenderer;        // 씬에 있는 스프라이트 참조
    public bool sceneSpriteVisibleOnlyThisLine;
}



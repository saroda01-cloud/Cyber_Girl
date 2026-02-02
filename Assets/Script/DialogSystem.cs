using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogSystem : MonoBehaviour
{
    [SerializeField]
    private Speaker[] speakers;                 // 대화에 참여하는 캐릭터들의 UI 배열
    [SerializeField]
    private DialogData[] dialogs;               // 현재 분기의 대사 목록 배열
    [SerializeField]
    private bool isAutoStart = true;            // 자동 시작 여부

    private bool isFirst = true;                // 최초 1회만 호출하기 위한 변수
    private int currentDialogIndex = -1;        // 현재 대사 순번
    private int currentSpeakerIndex = 0;        // 현재 말을 하는 화자(Speaker)의 speakers 배열 순번
    private float typingSpeed = 0.1f;           // 텍스트 타이핑 효과의 재생 속도
    private bool isTypingEffect = false;        // 텍스트 타이핑 효과를 재생중인지


    [Header("Day1~2: 글자 대신 Sprite 대사")]
    [SerializeField] private bool useSpriteDialogue = false; // true면 DialogData.dialogueSprite를 사용

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
            // 캐릭터 이미지는 보이도록 설정
            speakers[i].spriteRenderer.gameObject.SetActive(true);
        }
    }

    public bool UpdateDialog()
    {
        // 대사 분기가 시작될 때 1회만 호출
        if (isFirst == true)
        {
            Setup();

            if (isAutoStart) SetNextDialog();

            isFirst = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))

        {
            // 텍스트 타이핑 효과 재생중일 때 클릭하면 즉시 완성 텍스트 출력
            if (isTypingEffect == true)
            {
                isTypingEffect = false;

                StopCoroutine("OnTypingText");
                speakers[currentSpeakerIndex].textDialogue.text = dialogs[currentDialogIndex].dialogue;
                speakers[currentSpeakerIndex].objectArrow.SetActive(true);

                return false;
            }

            // 다음 대사 진행
            if (dialogs.Length > currentDialogIndex + 1)
            {
                SetNextDialog();
            }
            else
            {
                // 종료: UI 비활성화
                for (int i = 0; i < speakers.Length; ++i)
                {
                    SetActiveObjects(speakers[i], false);
                    speakers[i].spriteRenderer.gameObject.SetActive(true);
                }

                return true;
            }
        }

        return false;
    }

    private void SetNextDialog()
    {
        // 이전 화자 UI 끄기
        SetActiveObjects(speakers[currentSpeakerIndex], false);

        // 다음 대사로
        currentDialogIndex++;

        // 현재 화자 설정
        currentSpeakerIndex = dialogs[currentDialogIndex].speakerIndex;

        // 현재 화자 UI 켜기
        SetActiveObjects(speakers[currentSpeakerIndex], true);

        // Day1~2 + Sprite가 있는 경우
        if (useSpriteDialogue && dialogs[currentDialogIndex].dialogueSprite != null)
        {
            // Sprite 표시
            speakers[currentSpeakerIndex].imageDialogueSprite.sprite =
                dialogs[currentDialogIndex].dialogueSprite;

            // 타이핑 없이 바로 완료
            speakers[currentSpeakerIndex].objectArrow.SetActive(true);
        }
        else
        {
            // Sprite가 없으면 → 텍스트 출력 (Day1~2 포함)
            StartCoroutine(OnTypingText());
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

        // Sprite / Text 분기
        if (speaker.imageDialogueSprite != null)
            speaker.imageDialogueSprite.gameObject.SetActive(visible && hasSprite);

        speaker.textDialogue.gameObject.SetActive(visible && !hasSprite);

        // 화살표는 항상 꺼진 상태에서 시작
        speaker.objectArrow.SetActive(false);
    }


    private IEnumerator OnTypingText()
    {
        int index = 0;
        isTypingEffect = true;

        // 한글자씩 타이핑
        while (index < dialogs[currentDialogIndex].dialogue.Length)
        {
            speakers[currentSpeakerIndex].textDialogue.text =
                dialogs[currentDialogIndex].dialogue.Substring(0, index+1);

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

    // (선택) 하루마다 대사 데이터 자체를 바꿔 끼우고 싶을 때 사용
    public void SetDialogs(DialogData[] newDialogs)
    {
        dialogs = newDialogs;
        ResetDialog();
    }

    // Day별로 텍스트/스프라이트 출력 모드를 바꾸고 싶을 때 사용
    public void SetUseSpriteDialogue(bool value)
    {
        useSpriteDialogue = value;
    }
}

[System.Serializable]
public struct Speaker
{
    public SpriteRenderer spriteRenderer;       // 캐릭터 이미지
    public Image imageDialog;                   // 대화창 Image UI
    public TextMeshProUGUI textDialogue;        // 텍스트 대사
    public Image imageDialogueSprite;           // (Day1~2) 대사 스프라이트를 보여줄 Image UI
    public GameObject objectArrow;              // 대사가 완료되었을 때 출력되는 커서
}

[System.Serializable]
public struct DialogData
{
    public int speakerIndex;                    // 화자 index
    public string name;                         // 캐릭터 이름

    [TextArea(3, 5)]
    public string dialogue;                     // 대사(텍스트)

    public Sprite dialogueSprite;               // Day1~2에서 사용할 대사 스프라이트 (없으면 텍스트로 폴백)
}



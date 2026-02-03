using System.Collections;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    [Header("프레임 스프라이트")]
    [SerializeField] private SpriteRenderer smallFrameSprite;
    [SerializeField] private SpriteRenderer bigFrameSprite;
    [SerializeField] private SpriteRenderer verySmallFrameSprite;

    [Header("카메라")]
    [SerializeField] private Camera mainCamera;

    [Header("대화 시스템")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject spacebarIcon;

    [Header("Day별 대화 내용")]
    [SerializeField] private string[] day1Dialogues;
    [SerializeField] private string[] day2Dialogues;
    [SerializeField] private string[] day3Dialogues;
    [SerializeField] private string[] day4Dialogues;
    [SerializeField] private string[] day5Dialogues;
    [SerializeField] private string[] day6Dialogues;

    [Header("Day별 스프라이트 시퀀스")]
    [SerializeField] private GameObject[] day1Sprites;
    [SerializeField] private GameObject[] day2Sprites;
    [SerializeField] private GameObject[] day3Sprites;
    [SerializeField] private GameObject[] day4Sprites;
    [SerializeField] private GameObject[] day5Sprites;
    [SerializeField] private GameObject[] day6Sprites;

    [Header("타이밍 설정")]
    [SerializeField] private float smallFrameWaitTime = 3f;
    [SerializeField] private float zoomOutDuration = 2f;
    [SerializeField] private float zoomInDuration = 1f;
    [SerializeField] private float waitAfterZoomIn = 1f;
    [SerializeField] private float spriteDisplayTime = 2f;
    [SerializeField] private float waitBeforeBlink = 2f;


    [Header("점프 애니메이션")]
    [SerializeField] private JumpAnimation jumpAnimation; // Inspector에서 할당
    [SerializeField] private float jumpAnimationDuration = 3f; // 점프 애니메이션 총 시간
    [Header("줌아웃 시 활성화할 오브젝트")]
    [SerializeField] private GameObject objectToActivateOnZoomOut;
    [SerializeField] private float blinkDuration = 2f; // 깜박이는 총 시간
    [SerializeField] private float blinkInterval = 0.2f; // 깜박임 간격 (켜짐/꺼짐 전환 속도)

    private GameObject[] currentSpriteSequence;
    private bool canGoToNextScene = false;
    private Coroutine blinkCoroutine;

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (spacebarIcon != null)
            spacebarIcon.SetActive(false);

        DeactivateAllSprites();

        StartCoroutine(PlayCutscene());
    }

    private void DeactivateAllSprites()
    {
        GameObject[][] allSprites = { day1Sprites, day2Sprites, day3Sprites, day4Sprites, day5Sprites, day6Sprites };

        foreach (GameObject[] spriteArray in allSprites)
        {
            if (spriteArray != null)
            {
                foreach (GameObject sprite in spriteArray)
                {
                    if (sprite != null)
                        sprite.SetActive(false);
                }
            }
        }
    }

    void Update()
    {
        if (canGoToNextScene && Input.GetKeyDown(KeyCode.Space))
        {
            int currentDay = SceneTransitionManager.Instance.currentDay;

            if (currentDay == 1)
            {
                SceneTransitionManager.Instance.LoadNextMap();
            }
        }
    }

    private IEnumerator PlayCutscene()
    {
        int currentDay = SceneTransitionManager.Instance.currentDay;

        if (currentDay == 1)
        {
            SetCameraToSprite(smallFrameSprite);
            Debug.Log("작은 프레임에 줌인 완료");

            if (jumpAnimation != null)
            {
                yield return new WaitForSeconds(jumpAnimationDuration);
            }

            yield return new WaitForSeconds(smallFrameWaitTime);
            Debug.Log("줌아웃 시작!");

            // 줌아웃과 오브젝트 깜박임을 동시에 시작
            Coroutine zoomCoroutine = StartCoroutine(ZoomOutToSprite(bigFrameSprite));

            if (objectToActivateOnZoomOut != null)
            {
                StartCoroutine(BlinkThenActivateObject());
            }

            // 줌아웃 완료 대기
            yield return zoomCoroutine;
            Debug.Log("줌아웃 완료!");
        }
        else
        {
            SetCameraToSprite(bigFrameSprite);
            Debug.Log($"Day {currentDay}: 줌 연출 없이 바로 대화 시작");
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        dialogueManager.OnDialogueComplete = OnDialogueFinished;
        SetDialoguesByDay();
        dialogueManager.StartDialogue();
    }

    // 깜박이다가 완전히 켜지는 함수
    private IEnumerator BlinkThenActivateObject()
    {
        if (objectToActivateOnZoomOut == null) yield break;

        float elapsedTime = 0f;
        bool isOn = false;

        Debug.Log($"{blinkDuration}초 동안 깜박임 시작!");

        // 지정된 시간 동안 깜박임
        while (elapsedTime < blinkDuration)
        {
            isOn = !isOn;
            objectToActivateOnZoomOut.SetActive(isOn);

            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        // 최종적으로 완전히 켜기
        objectToActivateOnZoomOut.SetActive(true);
        Debug.Log("깜박임 종료, 오브젝트 완전 활성화!");
    }
    // 새 함수 추가
    private IEnumerator ActivateObjectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (objectToActivateOnZoomOut != null)
        {
            objectToActivateOnZoomOut.SetActive(true);
            Debug.Log($"줌아웃 시작 {delay}초 후 오브젝트 활성화!");
        }
    }
    private void SetDialoguesByDay()
    {
        int currentDay = SceneTransitionManager.Instance.currentDay;

        switch (currentDay)
        {
            case 1:
                dialogueManager.SetDialogues(day1Dialogues);
                currentSpriteSequence = day1Sprites;
                break;
            case 2:
                dialogueManager.SetDialogues(day2Dialogues);
                currentSpriteSequence = day2Sprites;
                break;
            case 3:
                dialogueManager.SetDialogues(day3Dialogues);
                currentSpriteSequence = day3Sprites;
                break;
            case 4:
                dialogueManager.SetDialogues(day4Dialogues);
                currentSpriteSequence = day4Sprites;
                break;
            case 5:
                dialogueManager.SetDialogues(day5Dialogues);
                currentSpriteSequence = day5Sprites;
                break;
            case 6:
                dialogueManager.SetDialogues(day6Dialogues);
                currentSpriteSequence = day6Sprites;
                break;
            default:
                dialogueManager.SetDialogues(day1Dialogues);
                currentSpriteSequence = day1Sprites;
                break;
        }

        Debug.Log($"Day {currentDay} 대화 및 스프라이트 설정 완료");
    }

    private void OnDialogueFinished()
    {
        Debug.Log("대화 완료! 대화창 제거 및 스프라이트 시퀀스 시작");

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        StartCoroutine(ShowSpriteSequence());
    }

    private IEnumerator ShowSpriteSequence()
    {
        SpriteRenderer targetFrame = verySmallFrameSprite != null ? verySmallFrameSprite : smallFrameSprite;

        yield return StartCoroutine(ZoomInToSprite(targetFrame));
        Debug.Log("스프라이트 시퀀스용 줌인 완료");

        yield return new WaitForSeconds(waitAfterZoomIn);
        Debug.Log("줌인 후 대기 완료, 스프라이트 시퀀스 시작");

        if (currentSpriteSequence != null)
        {
            for (int i = 0; i < currentSpriteSequence.Length; i++)
            {
                if (currentSpriteSequence[i] != null)
                {
                    currentSpriteSequence[i].SetActive(true);
                    yield return new WaitForSeconds(spriteDisplayTime);
                }
            }
        }

        // 점프 애니메이션 부분 삭제됨

        int currentDay = SceneTransitionManager.Instance.currentDay;

        if (currentDay == 1)
        {
            yield return new WaitForSeconds(waitBeforeBlink);

            if (spacebarIcon != null)
            {
                spacebarIcon.SetActive(true);
                StartBlinking();
            }

            canGoToNextScene = true;
        }
        else
        {
            Debug.Log($"Day {currentDay}: 스프라이트 시퀀스 완료, 바로 다음 씬으로 이동");
            yield return new WaitForSeconds(waitBeforeBlink);

            if (currentDay >= 6)
            {
                Debug.Log("게임 완료! 엔딩으로 이동");
                // SceneTransitionManager.Instance.LoadScene("Ending");
            }
            else
            {
                SceneTransitionManager.Instance.LoadNextMap();
            }
        }
    }
    private void SetCameraToSprite(SpriteRenderer targetSprite)
    {
        Vector3 spritePos = targetSprite.transform.position;
        mainCamera.transform.position = new Vector3(spritePos.x, spritePos.y, mainCamera.transform.position.z);

        Bounds spriteBounds = targetSprite.bounds;
        float cameraSize = Mathf.Max(spriteBounds.size.x / (2f * mainCamera.aspect), spriteBounds.size.y / 2f);
        mainCamera.orthographicSize = cameraSize;
    }

    private IEnumerator ZoomOutToSprite(SpriteRenderer targetSprite)
    {
        Vector3 startPos = mainCamera.transform.position;
        float startSize = mainCamera.orthographicSize;

        Vector3 spritePos = targetSprite.transform.position;
        Vector3 targetPos = new Vector3(spritePos.x, spritePos.y, mainCamera.transform.position.z);

        Bounds spriteBounds = targetSprite.bounds;
        float targetSize = Mathf.Max(spriteBounds.size.x / (2f * mainCamera.aspect), spriteBounds.size.y / 2f);

        float elapsedTime = 0f;

        while (elapsedTime < zoomOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / zoomOutDuration;

            t = Mathf.SmoothStep(0f, 1f, t);

            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.orthographicSize = targetSize;
    }

    private IEnumerator ZoomInToSprite(SpriteRenderer targetSprite)
    {
        Vector3 startPos = mainCamera.transform.position;
        float startSize = mainCamera.orthographicSize;

        Vector3 spritePos = targetSprite.transform.position;
        Vector3 targetPos = new Vector3(spritePos.x, spritePos.y, mainCamera.transform.position.z);

        Bounds spriteBounds = targetSprite.bounds;
        float targetSize = Mathf.Max(spriteBounds.size.x / (2f * mainCamera.aspect), spriteBounds.size.y / 2f);

        float elapsedTime = 0f;

        while (elapsedTime < zoomInDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / zoomInDuration;

            t = Mathf.SmoothStep(0f, 1f, t);

            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.orthographicSize = targetSize;
    }

    private void StartBlinking()
    {
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkSpacebarIcon());
    }

    private IEnumerator BlinkSpacebarIcon()
    {
        UnityEngine.UI.Image iconImage = spacebarIcon.GetComponent<UnityEngine.UI.Image>();
        if (iconImage == null) yield break;

        while (spacebarIcon.activeInHierarchy)
        {
            yield return StartCoroutine(FadeIcon(iconImage, 1f, 0.3f, 0.5f));
            yield return StartCoroutine(FadeIcon(iconImage, 0.3f, 1f, 0.5f));
        }
    }

    private IEnumerator FadeIcon(UnityEngine.UI.Image iconImage, float fromAlpha, float toAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color originalColor = iconImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / duration);
            iconImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        iconImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, toAlpha);
    }
}
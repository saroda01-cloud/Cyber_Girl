using System.Collections;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    [Header("프레임 스프라이트")]
    [SerializeField] private SpriteRenderer smallFrameSprite;
    [SerializeField] private SpriteRenderer bigFrameSprite;

    [Header("카메라")]
    [SerializeField] private Camera mainCamera;

    [Header("대화 시스템")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject spacebarIcon;

    [Header("Day별 대화 내용")]
    [SerializeField] private string[] day1Dialogues; // First 시작
    [SerializeField] private string[] day2Dialogues; // Map1 완료 후
    [SerializeField] private string[] day3Dialogues; // Map2 완료 후
    [SerializeField] private string[] day4Dialogues; // Map3 완료 후
    [SerializeField] private string[] day5Dialogues; // Map4 완료 후
    [SerializeField] private string[] day6Dialogues; // Map5 완료 후 (마지막)

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

    private GameObject[] currentSpriteSequence;
    private bool canGoToNextScene = false;
    private Coroutine blinkCoroutine;

    void Start()
    {
        // 시작 시 모든 UI 요소 비활성화
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (spacebarIcon != null)
            spacebarIcon.SetActive(false);

        // 모든 Day의 스프라이트들 비활성화
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

            // Day 1에서만 스페이스바로 넘어가기 허용
            if (currentDay == 1)
            {
                SceneTransitionManager.Instance.LoadNextMap();
            }
        }
    }
    private IEnumerator PlayCutscene()
    {
        int currentDay = SceneTransitionManager.Instance.currentDay;

        if (currentDay == 1) // 첫 번째 방문만 줌 연출
        {
            // 1. 줌인
            SetCameraToSprite(smallFrameSprite);
            Debug.Log("작은 프레임에 줌인 완료");

            // 2. 대기
            yield return new WaitForSeconds(smallFrameWaitTime);
            Debug.Log("줌아웃 시작!");

            // 3. 줌아웃
            yield return StartCoroutine(ZoomOutToSprite(bigFrameSprite));
            Debug.Log("줌아웃 완료!");
        }
        else
        {
            // 두 번째 방문부터는 바로 큰 프레임으로 카메라 설정
            SetCameraToSprite(bigFrameSprite);
            Debug.Log($"Day {currentDay}: 줌 연출 없이 바로 대화 시작");
        }

        // 4. Day별 대화 시작
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        dialogueManager.OnDialogueComplete = OnDialogueFinished;
        SetDialoguesByDay();
        dialogueManager.StartDialogue();
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
        // 1. 다시 작은 프레임으로 줌인
        yield return StartCoroutine(ZoomInToSprite(smallFrameSprite));
        Debug.Log("스프라이트 시퀀스용 줌인 완료");

        // 2. 줌인 완료 후 대기
        yield return new WaitForSeconds(waitAfterZoomIn);
        Debug.Log("줌인 후 대기 완료, 스프라이트 시퀀스 시작");

        // 3. 현재 Day의 스프라이트들 순서대로 표시 (누적)
        if (currentSpriteSequence != null)
        {
            for (int i = 0; i < currentSpriteSequence.Length; i++)
            {
                if (currentSpriteSequence[i] != null)
                {
                    currentSpriteSequence[i].SetActive(true);
                    Debug.Log($"Day {SceneTransitionManager.Instance.currentDay} 스프라이트 {i + 1} 표시");
                    yield return new WaitForSeconds(spriteDisplayTime);
                }
            }
        }

        // 4. Day별 다른 처리
        int currentDay = SceneTransitionManager.Instance.currentDay;

        if (currentDay == 1) // 첫 번째만 스페이스바로 대기
        {
            // 모든 스프라이트 표시 완료 후 대기
            Debug.Log("모든 스프라이트 표시 완료! 스페이스바 표시까지 대기");
            yield return new WaitForSeconds(waitBeforeBlink);

            // 스페이스바 아이콘 활성화 및 깜빡임 시작
            if (spacebarIcon != null)
            {
                spacebarIcon.SetActive(true);
                StartBlinking();
            }

            canGoToNextScene = true;
            Debug.Log("스페이스바를 눌러 다음으로 이동하세요.");
        }
        else // 두 번째부터는 바로 다음 씬으로
        {
            Debug.Log($"Day {currentDay}: 스프라이트 시퀀스 완료, 바로 다음 씬으로 이동");
            yield return new WaitForSeconds(waitBeforeBlink); // 잠깐 대기 후

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
        // 스프라이트 위치로 카메라 이동
        Vector3 spritePos = targetSprite.transform.position;
        mainCamera.transform.position = new Vector3(spritePos.x, spritePos.y, mainCamera.transform.position.z);

        // 스프라이트 크기에 맞게 카메라 크기 조정
        Bounds spriteBounds = targetSprite.bounds;
        float cameraSize = Mathf.Max(spriteBounds.size.x / (2f * mainCamera.aspect), spriteBounds.size.y / 2f);
        mainCamera.orthographicSize = cameraSize;
    }

    private IEnumerator ZoomOutToSprite(SpriteRenderer targetSprite)
    {
        Vector3 startPos = mainCamera.transform.position;
        float startSize = mainCamera.orthographicSize;

        // 목표 위치와 크기 계산
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
            // 페이드 아웃
            yield return StartCoroutine(FadeIcon(iconImage, 1f, 0.3f, 0.5f));
            // 페이드 인
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
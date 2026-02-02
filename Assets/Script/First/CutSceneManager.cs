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
    [SerializeField] private GameObject spacebarIcon; // 스페이스바 아이콘 따로 참조

    [Header("대화 내용")]
    [SerializeField] private string[] dialogues;

    [Header("스프라이트 시퀀스")]
    [SerializeField] private GameObject[] spriteSequence; // 순서대로 띄울 스프라이트들
    [SerializeField] private float spriteDisplayTime = 2f; // 각 스프라이트 간격 (이미 있음)
    [SerializeField] private float waitAfterZoomIn = 1f; // 줌인 완료 후 대기 시간 (새로 추가!)
    [SerializeField] private float zoomInDuration = 1f; // 줌인 시간
    [SerializeField] private float waitBeforeBlink = 2f; // 모든 스프라이트 표시 후 대기 시간

    [Header("타이밍")]
    [SerializeField] private float smallFrameWaitTime = 3f;
    [SerializeField] private float zoomOutDuration = 2f;

    private bool canGoToNextScene = false;
    private Coroutine blinkCoroutine;

    void Start()
    {
        // 시작 시 대화창과 모든 시퀀스 스프라이트, 스페이스바 아이콘 비활성화
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (spacebarIcon != null)
            spacebarIcon.SetActive(false);

        foreach (GameObject sprite in spriteSequence)
        {
            if (sprite != null)
                sprite.SetActive(false);
        }

        StartCoroutine(PlayCutscene());
    }

    void Update()
    {
        // 스프라이트 시퀀스 완료 후 스페이스바로 다음 씬
        if (canGoToNextScene && Input.GetKeyDown(KeyCode.Space))
        {
            SceneTransitionManager.Instance.LoadNextMap();
        }
    }

    private IEnumerator PlayCutscene()
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

        // 4. 대화 시작
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        dialogueManager.OnDialogueComplete = OnDialogueFinished;
        dialogueManager.SetDialogues(dialogues);
        dialogueManager.StartDialogue();
    }

    private void OnDialogueFinished()
    {
        Debug.Log("대화 완료! 대화창 제거 및 스프라이트 시퀀스 시작");

        // 1. 대화창 비활성화
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        StartCoroutine(ShowSpriteSequence());
    }

    private IEnumerator ShowSpriteSequence()
    {
        // 1. 다시 작은 프레임으로 줌인
        yield return StartCoroutine(ZoomInToSprite(smallFrameSprite));
        Debug.Log("스프라이트 시퀀스용 줌인 완료");

        // 2. 줌인 완료 후 대기 (새로 추가!)
        yield return new WaitForSeconds(waitAfterZoomIn);
        Debug.Log("줌인 후 대기 완료, 스프라이트 시퀀스 시작");

        // 3. 스프라이트들 순서대로 표시 (누적)
        for (int i = 0; i < spriteSequence.Length; i++)
        {
            if (spriteSequence[i] != null)
            {
                spriteSequence[i].SetActive(true);
                Debug.Log($"스프라이트 {i + 1} 표시");
                yield return new WaitForSeconds(spriteDisplayTime); // 각 스프라이트 간격
            }
        }

        // 4. 모든 스프라이트 표시 완료 후 대기
        Debug.Log("모든 스프라이트 표시 완료! 2초 후 스페이스바 표시");
        yield return new WaitForSeconds(waitBeforeBlink);

        // 5. 스페이스바 아이콘 활성화 및 깜빡임 시작
        if (spacebarIcon != null)
        {
            spacebarIcon.SetActive(true);
            StartBlinking();
        }

        canGoToNextScene = true;
        Debug.Log("스페이스바를 눌러 다음 씬으로 이동하세요.");
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

}
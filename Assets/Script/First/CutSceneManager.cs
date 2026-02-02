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
    [SerializeField] private GameObject dialoguePanel; // 대화창 패널

    [Header("대화 내용")]
    [SerializeField] private string[] dialogues; // 인스펙터에서 대화 내용 작성

    [Header("타이밍")]
    [SerializeField] private float smallFrameWaitTime = 3f;
    [SerializeField] private float zoomOutDuration = 2f;

    void Start()
    {
        // 시작 시 대화창 비활성화
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        // 1. 작은 스프라이트에 딱 맞게 카메라 설정
        SetCameraToSprite(smallFrameSprite);
        Debug.Log("작은 프레임에 줌인 완료");

        // 2. 작은 프레임 시간만큼 대기
        yield return new WaitForSeconds(smallFrameWaitTime);
        Debug.Log("줌아웃 시작!");

        // 3. 큰 스프라이트에 맞게 줌아웃
        yield return StartCoroutine(ZoomOutToSprite(bigFrameSprite));
        Debug.Log("줌아웃 완료!");

        // 4. 대화창 활성화 및 대화 시작
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        dialogueManager.OnDialogueComplete = OnDialogueFinished;
        dialogueManager.SetDialogues(dialogues); // 대화 내용 전달
        dialogueManager.StartDialogue();
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
    private void OnDialogueFinished()
    {
        Debug.Log("대화 완료! 다음 씬으로 이동");
        SceneTransitionManager.Instance.LoadNextMap();
    }

}
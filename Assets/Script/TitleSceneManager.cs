using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleSceneManager : MonoBehaviour
{
    [Header("UI 텍스트")]
    [SerializeField] private TextMeshProUGUI instructionText;

    [Header("텍스트 설정")]
    [SerializeField] private string firstMessage = "PRESS ANY KEY TO CONTINUE";
    [SerializeField] private string secondMessage = "PRESS ONE MORE TIME TO START";
    [SerializeField] private float blinkSpeed = 0.5f;

    private bool firstKeyPressed = false;
    private Coroutine blinkCoroutine;

    void Start()
    {
        if (instructionText != null)
        {
            instructionText.text = firstMessage;
            StartBlinking();
        }
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (!firstKeyPressed)
            {
                // 첫 번째 키 입력
                OnFirstKeyPress();
            }
            else
            {
                // 두 번째 키 입력 - 게임 시작
                StartGame();
            }
        }
    }

    private void OnFirstKeyPress()
    {
        firstKeyPressed = true;

        // 블링크 중지
        StopBlinking();

        // 텍스트 변경 및 굵게 표시
        if (instructionText != null)
        {
            instructionText.text = secondMessage;
            instructionText.fontStyle = FontStyles.Bold; // 굵은 글씨
            instructionText.color = Color.white; // 완전히 보이게
        }

    }

    private void StartGame()
    {

        // SceneTransitionManager가 있다면 Day 1로 초기화
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.currentDay = 1;
        }

        SceneManager.LoadScene("First");
    }

    private void StartBlinking()
    {
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkText());
    }

    private void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
    }

    private IEnumerator BlinkText()
    {
        while (instructionText != null)
        {
            // 완전히 꺼짐 (알파 0)
            yield return StartCoroutine(FadeText(1f, 0f));
            // 완전히 켜짐 (알파 1)
            yield return StartCoroutine(FadeText(0f, 1f));
        }
    }

    private IEnumerator FadeText(float fromAlpha, float toAlpha)
    {
        float elapsedTime = 0f;
        Color originalColor = instructionText.color;

        while (elapsedTime < blinkSpeed)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / blinkSpeed);
            instructionText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // 최종 알파값 확실히 적용
        instructionText.color = new Color(originalColor.r, originalColor.g, originalColor.b, toAlpha);
    }
}
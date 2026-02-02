using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleAnyKeyToStart : MonoBehaviour
{
    [Header("전환 방식")]
    [Tooltip("체크하면 지정한 씬 이름으로 이동, 아니면 Build Index + 1로 이동")]
    [SerializeField] private bool loadBySceneName = false;

    [Tooltip("loadBySceneName이 true일 때 이동할 씬 이름 (Build Settings에 등록되어 있어야 함)")]
    [SerializeField] private string nextSceneName = "Game";

    [Header("옵션")]
    [Tooltip("키 연타 방지")]
    [SerializeField] private bool preventMultipleLoads = true;

    [Tooltip("전환 전에 잠깐 딜레이(초)")]
    [SerializeField] private float delaySeconds = 0f;

    private bool _loading = false;

    void Update()
    {
        if (preventMultipleLoads && _loading) return;

        // Legacy Input Manager 기준: 키보드/마우스 버튼/조이스틱 버튼 등 "아무 입력"
        if (Input.anyKeyDown)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        if (preventMultipleLoads) _loading = true;

        if (delaySeconds > 0f)
            Invoke(nameof(LoadNextScene), delaySeconds);
        else
            LoadNextScene();
    }

    private void LoadNextScene()
    {
        if (loadBySceneName)
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentIndex + 1);
        }
    }
}


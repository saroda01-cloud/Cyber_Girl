using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSceneController : MonoBehaviour
{
    [Header("타이틀 씬 이름")]
    [SerializeField] private string titleSceneName = "Title"; // 또는 "MainMenu", "Start" 등

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("[EndingScene] ESC 눌림! 타이틀로 이동");
            GoToTitle();
        }
    }

    void GoToTitle()
    {
        // SceneTransitionManager 초기화 (Day 1로 리셋)
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.currentDay = 1;
            Debug.Log("[EndingScene] SceneTransitionManager Day 리셋");
        }

        // 타이틀 씬 로드
        SceneManager.LoadScene(titleSceneName);
    }
}
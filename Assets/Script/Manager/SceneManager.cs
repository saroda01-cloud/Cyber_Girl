using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("진행도")]
    public int currentDay = 1; // 1부터 시작

    void Awake()
    {
        Debug.Log("[SceneTransitionManager] Awake 시작!");

        if (Instance != null && Instance != this)
        {
            Debug.Log("[SceneTransitionManager] 이미 존재 - 현재 오브젝트 삭제");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[SceneTransitionManager] 싱글톤 생성 완료! Instance 설정됨");
    }

    public void OnMapCleared()
    {
        Debug.Log($"[SceneTransitionManager] OnMapCleared 호출됨! Instance는 null? {Instance == null}");
        LoadScene("Venue");
    }

    // Venue에서 First로 (Day 증가)
    public void ContinueFromVenue()
    {
        currentDay++; // Day 증가
        LoadScene("First");
    }

    public void LoadNextMap()
    {
        switch (currentDay)
        {
            case 1: LoadScene("Map1"); break;
            case 2: LoadScene("Map2"); break;
            case 3: LoadScene("Map3"); break;
            case 4: LoadScene("Map4"); break;
            case 5: LoadScene("Map5"); break;
            case 6: LoadScene("First"); break; // Map5 완료 후 다시 First로
            default:
                Debug.Log("모든 맵 클리어!");
                // LoadScene("Ending");
                break;
        }
    }

    // 씬 로드
    // LoadScene을 private에서 public으로 변경
    public void LoadScene(string sceneName)
    {
        Debug.Log($"씬 로드: {sceneName}, Current Day: {currentDay}");
        SceneManager.LoadScene(sceneName);
    }
    // 현재 Day 확인
    public int GetCurrentDay()
    {
        return currentDay;
    }
}
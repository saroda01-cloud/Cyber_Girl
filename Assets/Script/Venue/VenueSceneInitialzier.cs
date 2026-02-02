using UnityEngine;

public class VenueSceneInitializer : MonoBehaviour
{
    [SerializeField] private DialogTest dialogTest;

    void Awake()
    {
        if (dialogTest != null && SceneTransitionManager.Instance != null)
        {
            int day = SceneTransitionManager.Instance.GetCurrentDay();
            dialogTest.SetCurrentDay(day);
            Debug.Log($"[VenueSceneInitializer] DialogTest에 Day {day} 설정 완료");
        }
    }
}
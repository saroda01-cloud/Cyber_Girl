using UnityEngine;

public class GoalInteraction : MonoBehaviour
{
    [Header("상호작용 설정")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private GameObject interactionUI; // "F를 눌러 이동" UI (선택사항)

    private bool isPlayerInRange = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            if (interactionUI != null)
            {
                interactionUI.SetActive(true);
            }

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            if (interactionUI != null)
            {
                interactionUI.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactKey))
        {
            // null 체크 추가!
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.OnMapCleared();
            }
            else
            {
                Debug.LogError("SceneTransitionManager가 없습니다! First 씬부터 시작하세요.");
            }
        }
    }
}
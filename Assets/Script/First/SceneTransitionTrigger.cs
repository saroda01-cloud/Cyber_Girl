using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("상호작용 설정")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private GameObject interactionUI; // "F를 눌러 시작" UI (선택사항)

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

            Debug.Log("F를 눌러 맵으로 이동");
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
            // First에서 다음 맵으로 이동
            SceneTransitionManager.Instance.LoadNextMap();
        }
    }
}
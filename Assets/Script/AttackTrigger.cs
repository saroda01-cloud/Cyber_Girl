using UnityEngine;
using TMPro;

public class AttackTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject attackPrefab;
    public TextMeshProUGUI codeText;
    public Transform boxStopper;

    [Header("Settings")]
    public string attackCode = "M*cr%23";
    public float warningTime = 5f;
    public float attackXOffset = 0f;
    public float cameraSize = 5f; // 카메라 사이즈 고정값

    private bool isTriggered = false;
    private float timer = 0f;
    private bool isWarning = false;
    private GameObject player;

    void Start()
    {
        if (codeText != null)
        {
            codeText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isWarning)
        {
            timer += Time.deltaTime;

            if (codeText != null)
            {
                codeText.text = attackCode + "\n" + (warningTime - timer).ToString("F1") + "s";
            }

            if (timer >= warningTime)
            {
                SpawnAttack();
                isWarning = false;

                if (codeText != null)
                {
                    codeText.gameObject.SetActive(false);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            player = other.gameObject;
            StartWarning();
            GetComponent<Collider2D>().enabled = false;
        }
    }

    void StartWarning()
    {
        isWarning = true;
        timer = 0f;

        if (codeText != null)
        {
            codeText.gameObject.SetActive(true);
            codeText.text = attackCode;
        }
    }

    void SpawnAttack()
    {
        if (boxStopper == null || attackPrefab == null || player == null) return;

        // 임시 생성해서 박스 높이 계산
        Vector3 tempPos = new Vector3(0, -1000, 0);
        GameObject attack = Instantiate(attackPrefab, tempPos, Quaternion.identity);

        SpriteRenderer sr = attack.GetComponentInChildren<SpriteRenderer>();
        float boxHeight = sr != null ? sr.bounds.size.y : 10f;

        // 시작 Y 좌표 계산
        float cameraBottomY = Camera.main.transform.position.y - cameraSize;
        float startY = cameraBottomY - (boxHeight / 2f);

        // 종료 Y 좌표 계산 (박스 중심 기준)
        float endY = boxStopper.position.y - (boxHeight / 2f);

        // 정확한 위치로 이동
        attack.transform.position = new Vector3(
            player.transform.position.x + attackXOffset,
            startY,
            0f
        );

        // 타겟 설정
        Attack script = attack.GetComponent<Attack>();
        if (script != null)
        {
            script.SetTarget(endY);
        }
    }
}
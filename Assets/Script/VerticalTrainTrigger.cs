using UnityEngine;
using TMPro;

public class VerticalTrainTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject trainPrefab;
    public TextMeshProUGUI codeText;
    public Transform trainStarter;  // 기차가 시작할 X 위치

    [Header("Settings")]
    public string attackCode = "TRAIN";
    public float warningTime = 3f;
    public bool fromBottom = true;  // true: 아래→위, false: 위→아래
    public float trainSpeed = 20f;

    private bool isTriggered = false;
    private float timer = 0f;
    private bool isWarning = false;

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
                SpawnTrain();
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

    void SpawnTrain()
    {
        if (trainPrefab == null || trainStarter == null)
        {
            Debug.LogError($"[{gameObject.name}] TrainPrefab or TrainStarter is missing!");
            return;
        }

        Camera cam = Camera.main;
        float cameraHalfHeight = cam.orthographicSize;  // 카메라 세로 절반 크기
        float cameraBottom = cam.transform.position.y - cameraHalfHeight;
        float cameraTop = cam.transform.position.y + cameraHalfHeight;

        // 임시 생성해서 기차 높이 계산
        Vector3 tempPos = new Vector3(-1000, -1000, 0);
        GameObject train = Instantiate(trainPrefab, tempPos, Quaternion.identity);
        SpriteRenderer sr = train.GetComponentInChildren<SpriteRenderer>();
        float trainHeight = sr != null ? sr.bounds.size.y : 5f;

        float startY, endY;

        if (fromBottom)
        {
            // 아래→위: 기차 위쪽 끝이 카메라 아래 밖
            startY = cameraBottom - (trainHeight / 2f);
            // 기차 아래쪽 끝이 카메라 위 밖
            endY = cameraTop + (trainHeight / 2f);
        }
        else
        {
            // 위→아래: 기차 아래쪽 끝이 카메라 위 밖
            startY = cameraTop + (trainHeight / 2f);
            // 기차 위쪽 끝이 카메라 아래 밖
            endY = cameraBottom - (trainHeight / 2f);
        }

        // TrainStarter의 X 좌표 사용
        float trainX = trainStarter.position.x;

        // 실제 위치로 이동
        train.transform.position = new Vector3(trainX, startY, 0f);

        Debug.Log($"Vertical Train spawned at ({trainX}, {startY}), moving to Y: {endY}");

        // VerticalTrain 스크립트에 목표 설정
        VerticalTrain script = train.GetComponentInChildren<VerticalTrain>();
        if (script != null)
        {
            script.SetTarget(endY, fromBottom, trainSpeed);
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] VerticalTrain script NOT found on prefab!");
        }
    }
}
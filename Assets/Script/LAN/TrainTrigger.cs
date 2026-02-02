using UnityEngine;
using TMPro;

public class TrainTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject trainPrefab;
    public TextMeshProUGUI codeText;
    public Transform trainStarter; // 기차가 지나갈 Y 높이를 지정하는 오브젝트

    [Header("Settings")]
    public string attackCode = "TRAIN";
    public float warningTime = 3f;
    public bool fromLeft = true; // true면 좌→우, false면 우→좌
    public float trainSpeed = 20f; // 열차 속도를 여기서 설정


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
            Debug.LogError("TrainPrefab or TrainStarter is missing!");
            return;
        }

        // TrainStarter Y 좌표 확인
        Debug.Log($"[TrainTrigger] TrainStarter Y: {trainStarter.position.y}");

        // 카메라 정보
        Camera cam = Camera.main;
        float cameraHalfWidth = cam.orthographicSize * cam.aspect;
        float cameraLeftX = cam.transform.position.x - cameraHalfWidth;
        float cameraRightX = cam.transform.position.x + cameraHalfWidth;

        // 임시 생성해서 기차 너비 계산
        Vector3 tempPos = new Vector3(-1000, -1000, 0);
        GameObject train = Instantiate(trainPrefab, tempPos, Quaternion.identity);
        SpriteRenderer sr = train.GetComponentInChildren<SpriteRenderer>();
        float trainWidth = sr != null ? sr.bounds.size.x : 5f;

        Debug.Log($"[TrainTrigger] Train Width: {trainWidth}");

        // 시작/종료 X 위치 계산
        float startX, endX;

        if (fromLeft)
        {
            startX = cameraLeftX - (trainWidth / 2f);
            endX = cameraRightX + (trainWidth / 2f);
        }
        else
        {
            startX = cameraRightX + (trainWidth / 2f);
            endX = cameraLeftX - (trainWidth / 2f);
        }

        // TrainStarter의 Y 좌표 사용
        float trainY = trainStarter.position.y;

        // 실제 위치로 이동
        train.transform.position = new Vector3(startX, trainY, 0f);

        Debug.Log($"[TrainTrigger] Train final position: ({train.transform.position.x}, {train.transform.position.y})");
        Debug.Log($"[TrainTrigger] Expected Y: {trainY}, Actual Y: {train.transform.position.y}");

        // Train 스크립트에 목표 설정
        Train script = train.GetComponentInChildren<Train>();
        if (script != null)
        {
            script.SetTarget(endX, fromLeft, trainSpeed); // 속도 전달!
        }
        else
        {
            Debug.LogError("[TrainTrigger] Train script NOT found on prefab!");
        }
    }
}
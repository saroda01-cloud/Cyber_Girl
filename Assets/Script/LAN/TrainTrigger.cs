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

        Camera cam = Camera.main;
        float cameraHalfWidth = cam.orthographicSize * cam.aspect;
        float cameraLeftX = cam.transform.position.x - cameraHalfWidth;
        float cameraRightX = cam.transform.position.x + cameraHalfWidth;

        Vector3 tempPos = new Vector3(-1000, -1000, 0);
        GameObject train = Instantiate(trainPrefab, tempPos, Quaternion.identity);
        SpriteRenderer sr = train.GetComponentInChildren<SpriteRenderer>();
        float trainWidth = sr != null ? sr.bounds.size.x : 5f;

        float startX, endX;
        if (fromLeft)
        {
            // 왼쪽에서 시작 → 오른쪽 끝 + 기차 전체 길이만큼 더 가야 사라짐
            startX = cameraLeftX - trainWidth;
            endX = cameraRightX + trainWidth;  // 기차 전체가 화면 밖으로
        }
        else
        {
            // 오른쪽에서 시작 → 왼쪽 끝 - 기차 전체 길이
            startX = cameraRightX + trainWidth;
            endX = cameraLeftX - trainWidth;  // 기차 전체가 화면 밖으로
        }

        float trainY = trainStarter.position.y;
        train.transform.position = new Vector3(startX, trainY, 0f);

        Train script = train.GetComponentInChildren<Train>();
        if (script != null)
        {
            script.SetTarget(endX, fromLeft, trainSpeed);
            Debug.Log($"[TrainTrigger] Train 목표: {endX}, 거리: {Mathf.Abs(endX - startX)}");
        }
        else
        {
            Debug.LogError("[TrainTrigger] Train script NOT found on prefab!");
        }
    }
}
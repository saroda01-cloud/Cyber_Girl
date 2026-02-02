using UnityEngine;
using TMPro;
using System.Collections;

public class TrainTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject trainPrefab;
    public TextMeshProUGUI codeText;
    public Transform trainStarter;

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip trainSoundClip;
    public float soundStartEarly = 1f; // 기차 나오기 몇 초 전에 소리 시작

    [Header("Settings")]
    public string attackCode = "TRAIN";
    public float warningTime = 3f;
    public bool fromLeft = true;
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
            StartCoroutine(TrainSequence());
            GetComponent<Collider2D>().enabled = false;
        }
    }

    private IEnumerator TrainSequence()
    {
        // 1. 경고 시작
        StartWarning();

        // 2. 기차 소리 시작할 타이밍까지 대기 (warningTime - soundStartEarly)
        float waitTimeForSound = warningTime - soundStartEarly;
        if (waitTimeForSound > 0)
        {
            yield return new WaitForSeconds(waitTimeForSound);
        }

        // 3. 기차 소리 재생 (기차 나오기 1초 전)
        if (audioSource != null && trainSoundClip != null)
        {
            audioSource.PlayOneShot(trainSoundClip);
            Debug.Log("기차 소리 시작!");
        }

        // 4. soundStartEarly 시간만큼 더 기다린 후 기차 생성
        yield return new WaitForSeconds(soundStartEarly);

        // 기존 SpawnTrain은 Update에서 처리되므로 여기서는 생략
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
            startX = cameraLeftX - trainWidth;
            endX = cameraRightX + trainWidth;
        }
        else
        {
            startX = cameraRightX + trainWidth;
            endX = cameraLeftX - trainWidth;
        }

        float trainY = trainStarter.position.y;
        train.transform.position = new Vector3(startX, trainY, 0f);

        Train script = train.GetComponentInChildren<Train>();
        if (script != null)
        {
            script.SetTarget(endX, fromLeft, trainSpeed);
        }
        else
        {
            Debug.LogError("[TrainTrigger] Train script NOT found on prefab!");
        }
    }
}
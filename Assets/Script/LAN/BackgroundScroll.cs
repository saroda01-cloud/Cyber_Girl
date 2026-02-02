using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public float backgroundWidth = 19.2f;
    public float backgroundScale = 2f;
    public Transform playerTransform;

    private Vector3 lastPosition;
    private float backgroundOffset;

    void Start()
    {
        lastPosition = playerTransform.position;
        backgroundOffset = backgroundWidth / 2f; // 초기 오프셋을 배경 너비의 절반으로 설정

        // 초기 배경 생성
        CreateBackgroundAt(-backgroundWidth / 2f); // 왼쪽 절반 지점에 배경 생성
        CreateBackgroundAt(backgroundWidth / 2f); // 오른쪽 절반 지점에 배경 생성
    }
    void Update()
    {
        // 플레이어의 이동 거리 계산
        float deltaMovement = playerTransform.position.x - lastPosition.x;
        backgroundOffset += deltaMovement;
        lastPosition = playerTransform.position;

        // 배경 위치 업데이트
        if (backgroundOffset > backgroundWidth)
        {
            backgroundOffset -= backgroundWidth;
            CreateBackgroundAt(lastPosition.x + backgroundWidth / 2f); // 오른쪽 절반 지점에 배경 생성
        }
        else if (backgroundOffset < 0f)
        {
            backgroundOffset += backgroundWidth;
            CreateBackgroundAt(lastPosition.x - backgroundWidth / 2f); // 왼쪽 절반 지점에 배경 생성
        }
    }
    void CreateBackgroundAt(float xPosition)
    {
        GameObject backgroundObject = new GameObject("Background");
        backgroundObject.transform.parent = transform;
        backgroundObject.transform.position = new Vector3(xPosition, 0f, 10f);
        backgroundObject.transform.localScale = Vector3.one * backgroundScale;

        SpriteRenderer spriteRenderer = backgroundObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetComponent<SpriteRenderer>().sprite;
    }
}
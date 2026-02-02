using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    [Header("배경 오브젝트")]
    [SerializeField] private Transform background0;
    [SerializeField] private Transform background1;

    [Header("카메라")]
    [SerializeField] private Transform cam;

    [Header("패럴랙스")]
    [SerializeField] private float parallaxSpeed = 0.5f;

    private float backgroundWidth;
    private float lastCamX;

    void Start()
    {
        // 카메라 자동 찾기
        if (cam == null)
        {
            cam = Camera.main.transform;
            Debug.Log("카메라 자동 할당 완료");
        }

        // 배경 자동 찾기 (할당 안 되어있으면)
        if (background0 == null || background1 == null)
        {
            Transform[] children = GetComponentsInChildren<Transform>();
            int bgIndex = 0;
            foreach (Transform child in children)
            {
                if (child == transform) continue; // 자기 자신 제외

                if (child.GetComponent<SpriteRenderer>() != null)
                {
                    if (bgIndex == 0) background0 = child;
                    else if (bgIndex == 1) background1 = child;
                    bgIndex++;
                }
            }
            Debug.Log($"배경 자동 할당: BG0={background0.name}, BG1={background1.name}");
        }

        SpriteRenderer sr = background0.GetComponent<SpriteRenderer>();
        backgroundWidth = sr.bounds.size.x;

        lastCamX = cam.position.x;

        background0.position = new Vector3(0, background0.position.y, background0.position.z);
        background1.position = new Vector3(backgroundWidth, background1.position.y, background1.position.z);
    }

    void Update()
    {
        float camDeltaX = cam.position.x - lastCamX;
        float parallaxMove = camDeltaX * parallaxSpeed;

        background0.position += Vector3.right * parallaxMove;
        background1.position += Vector3.right * parallaxMove;

        if (cam.position.x - background0.position.x >= backgroundWidth)
        {
            background0.position = new Vector3(
                background1.position.x + backgroundWidth,
                background0.position.y,
                background0.position.z
            );
        }

        if (cam.position.x - background1.position.x >= backgroundWidth)
        {
            background1.position = new Vector3(
                background0.position.x + backgroundWidth,
                background1.position.y,
                background1.position.z
            );
        }

        if (background0.position.x - cam.position.x >= backgroundWidth)
        {
            background0.position = new Vector3(
                background1.position.x - backgroundWidth,
                background0.position.y,
                background0.position.z
            );
        }

        if (background1.position.x - cam.position.x >= backgroundWidth)
        {
            background1.position = new Vector3(
                background0.position.x - backgroundWidth,
                background1.position.y,
                background1.position.z
            );
        }

        lastCamX = cam.position.x;
    }
}
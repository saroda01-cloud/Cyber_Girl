using UnityEngine;

public class PlayerZoomCamera : MonoBehaviour
{
    public Transform player;
    public Transform targetObject;

    public float minDistance = 5f;
    public float maxDistance = 10f;

    public float zoomInSize = 3f;  // 가까울 때
    public float zoomOutSize = 10f; // 멀 때

    public float smooth = 5f;

    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        float dist = Vector2.Distance(player.position, targetObject.position);

        // 0~1 범위로 정규화
        float t = Mathf.InverseLerp(minDistance, maxDistance, dist);

        // 가까울수록 zoomIn, 멀수록 zoomOut
        float targetSize = Mathf.Lerp(zoomInSize, zoomOutSize, t);

        // 부드럽게 이동
        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetSize,
            Time.deltaTime * smooth
        );
    }
}


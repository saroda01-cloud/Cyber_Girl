using UnityEngine;

public class PlayerZoomCamera : MonoBehaviour
{
    public Transform player;
    public Transform targetObject;

    [Header("Distance Zoom")]
    public float minDistance = 5f;
    public float maxDistance = 10f;

    public float zoomInSize = 3f;   // 가까울 때
    public float zoomOutSize = 10f; // 멀 때
    public float smooth = 5f;

    [Header("Ground Clamp")]
    public float groundY = 0f; // 바닥의 Y 좌표 (Tilemap 기준)

    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        //  거리 기반 줌
        float dist = Vector2.Distance(player.position, targetObject.position);
        float t = Mathf.InverseLerp(minDistance, maxDistance, dist);
        float targetSize = Mathf.Lerp(zoomInSize, zoomOutSize, t);

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetSize,
            Time.deltaTime * smooth
        );

        //  카메라 위치 보정
        Vector3 camPos = transform.position;

        // X는 플레이어 따라가기
        camPos.x = player.position.x;

        // 핵심: 카메라 하단이 바닥에 딱 맞도록
        camPos.y = groundY + cam.orthographicSize;

        transform.position = camPos;
    }
}



using UnityEngine;

public class PlayerZoomCamera : MonoBehaviour
{
    public Transform player;
    public Transform targetObject;

    [Header("Distance Zoom")]
    public float minDistance = 5f;
    public float maxDistance = 10f;

    public float zoomInSize = 3f;
    public float zoomOutSize = 10f;
    public float smooth = 5f;

    [Header("Ground Clamp")]
    public float groundY = 0f;

    [Header("Options")]
    public bool clampGroundY = true;     // Y 바닥 고정
    public bool keepLocalZ = true;       // Z 유지(2D 기본 -10)

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player == null || targetObject == null) return;

        // 1) 거리 기반 줌
        float dist = Vector2.Distance(player.position, targetObject.position);
        float t = Mathf.InverseLerp(minDistance, maxDistance, dist);
        float targetSize = Mathf.Lerp(zoomInSize, zoomOutSize, t);

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * smooth);

        // 2) 위치 보정
        // 카메라가 Player 자식일 때: 월드 position을 직접 만지지 말고 localPosition을 만진다
        if (transform.parent != null)
        {
            Vector3 lp = transform.localPosition;

            if (clampGroundY)
            {
                // 원하는 월드 Y = groundY + size
                float desiredWorldY = groundY + cam.orthographicSize;

                // 현재 카메라 월드 좌표를 desiredWorldY로 맞추기 위해,
                // desired 월드 위치를 부모 로컬로 변환해서 lp.y에 적용
                Vector3 desiredWorldPos = new Vector3(transform.position.x, desiredWorldY, transform.position.z);
                Vector3 desiredLocalPos = transform.parent.InverseTransformPoint(desiredWorldPos);

                lp.y = desiredLocalPos.y;
            }

            if (keepLocalZ)
            {
                lp.z = -10f;
            }

            transform.localPosition = lp;
        }
        else
        {
            // 부모가 없을 때만 기존 방식(월드) 사용
            Vector3 wp = transform.position;
            wp.x = player.position.x;
            wp.y = groundY + cam.orthographicSize;
            transform.position = wp;
        }
    }
}



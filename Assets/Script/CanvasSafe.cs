using UnityEngine;

public class CanvasSafeArea : MonoBehaviour
{
    private RectTransform rectTransform;
    private Camera cam;
    private Rect lastSafeArea;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        cam = Camera.main;
        lastSafeArea = new Rect(0, 0, 0, 0);
    }

    void Update()
    {
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        if (cam == null) return;

        // 카메라의 실제 viewport rect 가져오기
        Rect camRect = cam.rect;

        // 변경사항이 있을 때만 업데이트
        if (camRect == lastSafeArea) return;
        lastSafeArea = camRect;

        // Canvas RectTransform을 카메라 viewport에 맞춤
        rectTransform.anchorMin = new Vector2(camRect.x, camRect.y);
        rectTransform.anchorMax = new Vector2(camRect.x + camRect.width, camRect.y + camRect.height);

        // offset 초기화
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Debug.Log($"[CanvasSafeArea] Applied: anchor min={rectTransform.anchorMin}, max={rectTransform.anchorMax}");
    }
}
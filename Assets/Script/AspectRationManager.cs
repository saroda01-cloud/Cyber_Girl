using UnityEngine;

public class AspectRatioManager : MonoBehaviour
{
    [Header("목표 비율")]
    [SerializeField] private float targetAspect = 16f / 9f; // 16:9

    [Header("전체화면 설정")]
    [SerializeField] private bool forceFullscreen = true;
    [SerializeField] private int targetWidth = 1920;
    [SerializeField] private int targetHeight = 1080;

    private Camera cam;
    private int lastScreenWidth;
    private int lastScreenHeight;

    void Awake()
    {
        cam = Camera.main;

        if (forceFullscreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        ApplyAspectRatio();
    }

    void Update()
    {
        // 해상도 변경 감지
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            ApplyAspectRatio();
        }
    }

    void ApplyAspectRatio()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // 화면이 더 좁음 (세로로 긴 경우) - 위아래 레터박스
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        else
        {
            // 화면이 더 넓음 (가로로 긴 경우) - 좌우 필러박스
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }

        Debug.Log($"[AspectRatio] Screen: {Screen.width}x{Screen.height}, Aspect: {windowAspect:F2}, Target: {targetAspect:F2}");
    }
}
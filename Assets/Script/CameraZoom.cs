using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Camera cam;
    public TMPro.TextMeshProUGUI text;

    public float duration1 = 5f;
    public float duration2 = 2f;
    public float startOrthoSize = 1.5f;
    public float restOrthoSize = 3f;
    public float endOrthoSize = 5f;

    public bool rest = false;

    void Reset()
    {
        cam = Camera.main;
    }

    void Start()
    {
        if (cam == null) cam = Camera.main;

        cam.orthographic = true;
        cam.orthographicSize = startOrthoSize;

        StartCoroutine(ZoomSequence());
    }

    IEnumerator ZoomSequence()
    {
        if (rest)
        {
            // 1단계: start → rest
            yield return StartCoroutine(Zoom(startOrthoSize, restOrthoSize, duration1));

            yield return new WaitForSeconds(2f);
            // 2단계: rest → end
            yield return StartCoroutine(Zoom(restOrthoSize, endOrthoSize, duration2));
            text.gameObject.SetActive(false);
        }
        else
        {
            // 그냥 start → end
            yield return StartCoroutine(Zoom(startOrthoSize, endOrthoSize, duration1));
            text.gameObject.SetActive(false);
        }
    }

    IEnumerator Zoom(float from, float to, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);

            // Smooth easing
            p = p * p * (3f - 2f * p);

            cam.orthographicSize = Mathf.Lerp(from, to, p);

            yield return null;
        }

        cam.orthographicSize = to;
    }
}

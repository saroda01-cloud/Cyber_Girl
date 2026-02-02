using UnityEngine;

public class Attack : MonoBehaviour
{
    private float riseSpeed = 10f;

    private float targetY;
    private bool rising = true;
    private bool hasTarget = false;

    public void SetTarget(float stopCenterY, float speed) //  속도 파라미터 추가
    {
        targetY = stopCenterY;
        riseSpeed = speed; //  속도 설정
        hasTarget = true;
        Debug.Log($"[Attack] Target Set! Y: {transform.position.y} → {targetY}, Speed: {riseSpeed}");
    }

    void Update()
    {
        if (!hasTarget) return;

        if (rising)
        {
            transform.Translate(Vector2.up * riseSpeed * Time.deltaTime);

            if (transform.position.y >= targetY)
            {
                rising = false;
            }
        }
        else
        {
            transform.Translate(Vector2.down * riseSpeed * Time.deltaTime);

            if (transform.position.y < targetY - 15f)
            {
                Destroy(gameObject);
            }
        }
    }
}
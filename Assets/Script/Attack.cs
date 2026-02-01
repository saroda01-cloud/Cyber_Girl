using UnityEngine;

public class Attack : MonoBehaviour
{
    public float riseSpeed = 10f;

    private float targetY;
    private bool rising = true;
    private bool hasTarget = false;

    public void SetTarget(float stopCenterY)
    {
        targetY = stopCenterY;
        hasTarget = true;
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
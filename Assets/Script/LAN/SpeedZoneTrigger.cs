using UnityEngine;

public class SpeedZoneTrigger : MonoBehaviour
{
    [Header("Speed Settings")]
    public float newMoveSpeed = 10f;     // 변경할 이동속도
    public float defaultJump = 10f;

    [Header("Behavior")]
    public bool oneTime = true;          // 1회성 트리거 여부

    private bool used = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used && oneTime) return;

        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.moveSpeed = newMoveSpeed;
                pc.jumpForce = defaultJump;

                used = true;
            }
        }
    }
}
    
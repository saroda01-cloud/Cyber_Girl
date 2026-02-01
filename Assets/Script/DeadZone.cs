using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public Transform respawnPoint; // 이 데드존의 리스폰 위치

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamageAndRespawn(respawnPoint);
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Invincibility Settings")]
    public float invincibleTime = 1f;
    private bool isInvincible = false;

    [Header("Visual Feedback")]
    public SpriteRenderer playerSprite;
    public float blinkInterval = 0.1f;

    [Header("UI")]
    public GameObject[] healthIcons;

    private PlayerController playerController;

    void Start()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        bool showHealthUI = sceneName.StartsWith("Map");

        if (showHealthUI)
        {
            currentHealth = maxHealth; // 맵에서는 항상 만피로 시작
        }

        // UI 표시/숨김
        foreach (GameObject icon in healthIcons)
        {
            if (icon != null)
                icon.SetActive(showHealthUI);
        }

        UpdateHealthUI();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isInvincible)
        {
            Debug.Log("Invincible - ignoring damage");
            return;
        }

        if (other.CompareTag("Dead"))
        {
            Debug.Log($"Hit by {other.gameObject.name}!");
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (isInvincible)
        {
            Debug.Log("TakeDamage blocked by invincibility");
            return;
        }

        isInvincible = true;
        currentHealth--;
        Debug.Log($"Damage taken! Health: {currentHealth}");

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    // DeadZone 전용 - 무적 상태 무시
    public void TakeDamageAndRespawn(Transform respawnPoint)
    {
        // 무적 상태 강제 종료
        if (isInvincible)
        {
            StopCoroutine(InvincibilityCoroutine());
            if (playerSprite != null)
            {
                playerSprite.enabled = true;
            }
            isInvincible = false;
            Debug.Log("무적 상태 강제 해제됨 (DeadZone)");
        }

        isInvincible = true;
        currentHealth--;
        Debug.Log($"Damage taken! Health: {currentHealth}");

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Respawn(respawnPoint);
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    void Respawn(Transform respawnPoint)
    {
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            Debug.Log("Player respawned!");
        }
    }

    IEnumerator InvincibilityCoroutine()
    {
        Debug.Log("Invincibility started!");
        float elapsed = 0f;

        while (elapsed < invincibleTime)
        {
            if (playerSprite != null)
            {
                playerSprite.enabled = !playerSprite.enabled;
            }
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        if (playerSprite != null)
        {
            playerSprite.enabled = true;
        }

        isInvincible = false;
        Debug.Log("Invincibility ended");
    }

    void UpdateHealthUI()
    {
        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (healthIcons[i] != null)
            {
                healthIcons[i].SetActive(i < currentHealth);
            }
        }
    }

    void Die()
    {
        Debug.Log("Player Died! 체력 회복 후 First Scene으로 돌아가기");

        // 체력 완전 회복 (PlayerHealth의 변수들)
        currentHealth = maxHealth; // 3으로 회복
        UpdateHealthUI();

        // 무적 상태 해제
        isInvincible = false;
        if (playerSprite != null)
        {
            playerSprite.enabled = true;
        }

        // PlayerController.Die() 호출하지 않고 바로 씬 이동
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("First");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("First");
        }

        // PlayerController.Die()는 호출하지 않음!
    }
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }
}
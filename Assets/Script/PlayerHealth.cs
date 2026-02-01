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
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();

        if (playerSprite == null)
        {
            playerSprite = GetComponent<SpriteRenderer>();
            if (playerSprite == null)
            {
                playerSprite = GetComponentInChildren<SpriteRenderer>();
            }
        }

        UpdateHealthUI();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //  무적 상태 먼저 체크
        if (isInvincible)
        {
            Debug.Log("Invincible - ignoring damage");
            return;
        }

        if (other.CompareTag("Dead"))
        {
            Debug.Log($"Hit by {other.gameObject.name}! Current invincible: {isInvincible}");
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        //  이중 체크
        if (isInvincible)
        {
            Debug.Log("TakeDamage blocked by invincibility");
            return;
        }

        //  즉시 무적 설정 (코루틴 시작 전에!)
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
        if (playerController != null)
        {
            playerController.Die();
        }
        else
        {
            Debug.Log("Player Died!");
            Destroy(gameObject);
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }
}
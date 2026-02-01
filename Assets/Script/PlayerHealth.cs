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

    //  PlayerController 참조 추가
    private PlayerController playerController;

    void Start()
    {
        currentHealth = maxHealth;

        // PlayerController 가져오기
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
        if (other.CompareTag("Dead") && !isInvincible)
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        if (isInvincible) return;

        currentHealth--;
        Debug.Log($"Hit! Current Health: {currentHealth}");

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
        isInvincible = true;
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
        //  PlayerController의 Die() 호출
        if (playerController != null)
        {
            playerController.Die();
        }
        else
        {
            // PlayerController가 없으면 직접 처리
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
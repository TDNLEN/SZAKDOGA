using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 10;
    private int currentHealth;
    private bool isDead = false;

    [Header("UI References")]
    public Image healthBarFill;

    [Header("Sprite Flash")]
    public SpriteRenderer sprite;
    public Color hitColor = Color.red;
    public float hitFlashTime = 0.1f;
    private Color originalColor;

    [Header("Protection")]
    public bool invulnerable = false;
    public bool IsInvulnerable => invulnerable;

    [Header("Death")]
    public PlayerDeathHandler deathHandler;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip healSound;
    [Range(0f, 1f)] public float healVolume = 1f;
    public AudioClip damageSound;
    [Range(0f, 1f)] public float damageVolume = 1f;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();

        if (sprite != null)
            originalColor = sprite.color;

        if (deathHandler == null)
            deathHandler = GetComponent<PlayerDeathHandler>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        UpdateHealthBar();
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        if (invulnerable) return;
        if (dmg <= 0) return;

        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthBar();
        PlayDamageSound();
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
            Die();
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill == null) return;

        float t = Mathf.Clamp01((float)currentHealth / maxHealth);

        RectTransform rt = healthBarFill.rectTransform;
        rt.localScale = new Vector3(t, 1f, 1f);

        float offset = (1f - t) * (rt.rect.width / 2f);
        rt.localPosition = new Vector3(-offset, 0f, 0f);
    }

    private IEnumerator HitFlash()
    {
        if (sprite == null) yield break;

        sprite.color = hitColor;
        yield return new WaitForSeconds(hitFlashTime);
        sprite.color = originalColor;
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player meghalt!");

        if (deathHandler != null)
            deathHandler.Die();
    }

    public void Heal(int amount)
    {
        TryHeal(amount);
    }

    public bool IsFullHealth => !isDead && currentHealth >= maxHealth;

    public bool TryHeal(int amount)
    {
        if (isDead) return false;
        if (currentHealth >= maxHealth) return false;
        if (amount <= 0) return false;

        int oldHealth = currentHealth;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (currentHealth > oldHealth)
        {
            UpdateHealthBar();
            PlayHealSound();
            return true;
        }

        return false;
    }

    private void PlayHealSound()
    {
        if (audioSource == null || healSound == null)
            return;

        audioSource.PlayOneShot(healSound, healVolume);
    }

    private void PlayDamageSound()
    {
        if (audioSource == null || damageSound == null)
            return;

        audioSource.PlayOneShot(damageSound, damageVolume);
    }
}
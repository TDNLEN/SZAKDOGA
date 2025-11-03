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
    public Image healthBarFill; // ide húzd be a zöld UI Image-et (HealthBarFill)
    private float startWidth;

    [Header("Sprite Flash")]
    public SpriteRenderer sprite;  // ide húzd be a Player SpriteRenderer-t
    public Color hitColor = Color.red;
    public float hitFlashTime = 0.1f;
    private Color originalColor;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
            originalColor = sprite.color;

        if (healthBarFill != null)
            startWidth = healthBarFill.rectTransform.localScale.x;

        UpdateHealthBar();
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthBar();
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill == null) return;

        float t = (float)currentHealth / maxHealth;
        healthBarFill.rectTransform.localScale = new Vector3(t, 1f, 1f);

        // 🔹 Ezzel toljuk el balról jobbra
        float offset = (1f - t) * (healthBarFill.rectTransform.rect.width / 2f);
        healthBarFill.rectTransform.localPosition = new Vector3(-offset, 0, 0);
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
        isDead = true;
        Debug.Log("Player meghalt!");
        // később ide jöhet respawn / game over képernyő
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthBar();
    }
}

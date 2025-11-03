using System.Collections;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 5;
    private int currentHealth;
    private bool isDead = false;
    public bool IsDead => isDead; // <-- külső kódhoz

    [Header("Health bar")]
    public Transform barFill; // piros csík (Transform, nem UI)
    private float startScaleX;

    [Header("Animator")]
    public Animator animator;
    public string dieTriggerName = "Die";

    [Header("Hit flash")]
    public SpriteRenderer sprite;
    public Color hitColor = Color.red;
    public float hitFlashTime = 0.1f;
    private Color originalColor;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.1f;
    private Rigidbody2D rb;

    [Header("Cleanup")]
    public float destroyAfterDeath = 1.0f;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (barFill != null)
            startScaleX = barFill.localScale.x;

        if (animator == null) animator = GetComponent<Animator>();
        if (sprite == null) sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) originalColor = sprite.color;

        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int dmg, Vector2? hitSource = null)
    {
        if (isDead) return;

        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        UpdateBar();
        StartCoroutine(HitFlash());

        if (hitSource.HasValue)
            ApplyKnockback(hitSource.Value);

        if (currentHealth <= 0)
            Die();
    }

    private void UpdateBar()
    {
        if (barFill == null) return;

        float t = Mathf.Clamp01((float)currentHealth / maxHealth);
        barFill.localScale = new Vector3(startScaleX * t, barFill.localScale.y, barFill.localScale.z);

        float offset = (startScaleX - startScaleX * t) / 2f;
        barFill.localPosition = new Vector3(-offset, barFill.localPosition.y, barFill.localPosition.z);
    }

    private void Die()
    {
        isDead = true;

        var follow = GetComponent<EnemyFollow>();
        if (follow) follow.enabled = false;

        if (animator != null && !string.IsNullOrEmpty(dieTriggerName))
            animator.SetTrigger(dieTriggerName);

        Destroy(gameObject, destroyAfterDeath);
    }

    private IEnumerator HitFlash()
    {
        if (sprite == null) yield break;
        sprite.color = hitColor;
        yield return new WaitForSeconds(hitFlashTime);
        sprite.color = originalColor;
    }

    private void ApplyKnockback(Vector2 hitSource)
    {
        if (rb == null) return;

        Vector2 knockDir = ((Vector2)transform.position - hitSource).normalized;
        rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);

        StartCoroutine(StopKnockback());
    }

    private IEnumerator StopKnockback()
    {
        yield return new WaitForSeconds(knockbackDuration);
        if (rb != null) rb.linearVelocity = Vector2.zero; // <-- helyes property
    }

    // Ha anim eventtel akarsz törölni:
    public void OnDeathEnd() => Destroy(gameObject);
}

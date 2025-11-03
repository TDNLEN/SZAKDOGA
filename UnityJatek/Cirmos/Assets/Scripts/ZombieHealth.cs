using System.Collections;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 5;
    private int currentHealth;
    private bool isDead = false;

    [Header("Health bar")]
    public Transform barFill;          // a piros cs�k
    private float startScaleX;

    [Header("Animator")]
    public Animator animator;          // h�zd be a zombir�l
    public string dieTriggerName = "Die";

    [Header("Hit flash")]
    public SpriteRenderer sprite;      // h�zd be a zombir�l
    public Color hitColor = Color.red;
    public float hitFlashTime = 0.1f;
    private Color originalColor;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.1f;
    private Rigidbody2D rb;

    [Header("Cleanup")]
    public float destroyAfterDeath = 1.0f; // anim�ci� ut�n ennyi id�vel t�nj�n el

    private void Awake()
    {
        currentHealth = maxHealth;

        if (barFill != null)
            startScaleX = barFill.localScale.x;

        // ha nem h�ztad be inspectorban, pr�b�ljuk megkeresni
        if (animator == null)
            animator = GetComponent<Animator>();

        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();

        if (sprite != null)
            originalColor = sprite.color;

        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int dmg, Vector2? hitSource = null)
    {
        if (isDead) return; // ha m�r haldoklik, ne sebezzen tov�bb

        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        UpdateBar();

        // villan�s
        StartCoroutine(HitFlash());

        if (hitSource.HasValue)
            ApplyKnockback(hitSource.Value);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateBar()
    {
        if (barFill == null) return;

        float t = (float)currentHealth / maxHealth;

        // �j m�ret
        barFill.localScale = new Vector3(startScaleX * t, barFill.localScale.y, barFill.localScale.z);

        // �j poz�ci� � a bal oldalt r�gz�tj�k, �s jobbr�l r�vid�l
        float offset = (startScaleX - startScaleX * t) / 2f;
        barFill.localPosition = new Vector3(-offset, barFill.localPosition.y, barFill.localPosition.z);
    }


    private void Die()
    {
        isDead = true;

        // ha van enemy mozg�s scripted, itt letilthatod
        var follow = GetComponent<EnemyFollow>();
        if (follow) follow.enabled = false;

        // animatornak sz�lunk
        if (animator != null && !string.IsNullOrEmpty(dieTriggerName))
            animator.SetTrigger(dieTriggerName);

        // ha nem akarsz eventet, id� ut�n elt�ntetj�k
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
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
    // ha ink�bb anim�ci�s eventtel akarod elt�ntetni, akkor az anim klip v�g�n h�vd ezt:
    public void OnDeathEnd()
    {
        Destroy(gameObject);
    }
}

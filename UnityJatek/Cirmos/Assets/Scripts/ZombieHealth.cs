using System.Collections;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 5;
    private int currentHealth;
    private bool isDead = false;

    [Header("Health bar")]
    public Transform barFill;          // a piros csík
    private float startScaleX;

    [Header("Animator")]
    public Animator animator;          // húzd be a zombiról
    public string dieTriggerName = "Die";

    [Header("Hit flash")]
    public SpriteRenderer sprite;      // húzd be a zombiról
    public Color hitColor = Color.red;
    public float hitFlashTime = 0.1f;
    private Color originalColor;

    [Header("Cleanup")]
    public float destroyAfterDeath = 1.0f; // animáció után ennyi idõvel tûnjön el

    private void Awake()
    {
        currentHealth = maxHealth;

        if (barFill != null)
            startScaleX = barFill.localScale.x;

        // ha nem húztad be inspectorban, próbáljuk megkeresni
        if (animator == null)
            animator = GetComponent<Animator>();

        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();

        if (sprite != null)
            originalColor = sprite.color;
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return; // ha már haldoklik, ne sebezzen tovább

        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        UpdateBar();

        // villanás
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateBar()
    {
        if (barFill == null) return;

        float t = (float)currentHealth / maxHealth;

        // új méret
        barFill.localScale = new Vector3(startScaleX * t, barFill.localScale.y, barFill.localScale.z);

        // új pozíció — a bal oldalt rögzítjük, és jobbról rövidül
        float offset = (startScaleX - startScaleX * t) / 2f;
        barFill.localPosition = new Vector3(-offset, barFill.localPosition.y, barFill.localPosition.z);
    }


    private void Die()
    {
        isDead = true;

        // ha van enemy mozgás scripted, itt letilthatod
        var follow = GetComponent<EnemyFollow>();
        if (follow) follow.enabled = false;

        // animatornak szólunk
        if (animator != null && !string.IsNullOrEmpty(dieTriggerName))
            animator.SetTrigger(dieTriggerName);

        // ha nem akarsz eventet, idõ után eltüntetjük
        Destroy(gameObject, destroyAfterDeath);
    }

    private IEnumerator HitFlash()
    {
        if (sprite == null) yield break;

        sprite.color = hitColor;
        yield return new WaitForSeconds(hitFlashTime);
        sprite.color = originalColor;
    }

    // ha inkább animációs eventtel akarod eltüntetni, akkor az anim klip végén hívd ezt:
    public void OnDeathEnd()
    {
        Destroy(gameObject);
    }
}

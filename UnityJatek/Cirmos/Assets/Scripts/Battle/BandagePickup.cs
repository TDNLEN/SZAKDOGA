using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BandagePickup : MonoBehaviour
{
    public int healAmount = 10;   // ennyit gyógyít egy bandage

    private void Reset()
    {
        var c = GetComponent<Collider2D>();
        if (c) c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var health = other.GetComponent<PlayerHealth>();
        if (health == null)
            health = other.GetComponentInParent<PlayerHealth>();
        if (health == null) return;

        // TryHeal magában eldönti, hogy lehet-e gyógyítani
        if (health.TryHeal(healAmount))
        {
            Destroy(gameObject);
        }
    }
}

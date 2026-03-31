using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BandagePickup : MonoBehaviour
{
    public int healAmount = 10;

    private void Reset()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GetComponent<ItemPickUp>() != null)
            return;

        if (!other.CompareTag("Player")) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null)
            health = other.GetComponentInParent<PlayerHealth>();
        if (health == null) return;

        if (health.TryHeal(healAmount))
        {
            Destroy(gameObject);
        }
    }
}
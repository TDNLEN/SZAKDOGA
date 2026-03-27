using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[DisallowMultipleComponent]
public class ItemPickUp : MonoBehaviour
{
    [Header("Equip pose (in hand)")]
    public Vector3 equipLocalPosition = new Vector3(0.25f, -0.05f, 0f);
    public Vector3 equipLocalRotation = new Vector3(0, 0, -20f);
    public Vector3 equipLocalScale = Vector3.one;

    [Header("Pickup control")]
    public float rePickupDelay = 0.25f;

    private bool picked = false;
    private float nextPickupTime = 0f;

    private void Reset()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    private void OnValidate()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time < nextPickupTime) return;
        if (picked) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null)
            health = other.GetComponentInParent<PlayerHealth>();

        // Heal item speciális logika:
        // ha NEM full HP-n van, akkor azonnal gyógyít és NEM kerül inventoryba
        HealPickupItem healPickup = GetComponent<HealPickupItem>();
        if (healPickup != null && health != null && !health.IsFullHealth)
        {
            bool healed = health.TryHeal(healPickup.healAmount);
            if (healed)
            {
                Destroy(gameObject);
                return;
            }
        }

        PlayerInventory inv = other.GetComponent<PlayerInventory>();
        if (inv == null)
            inv = other.GetComponentInParent<PlayerInventory>();
        if (inv == null) return;

        if (inv.TryAddItem(gameObject))
        {
            picked = true;
        }
    }

    public void OnDropped()
    {
        picked = false;
        nextPickupTime = Time.time + rePickupDelay;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = true;
        }
    }
}
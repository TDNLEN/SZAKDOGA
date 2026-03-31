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

    [Header("Pickup Audio")]
    public AudioClip pickupSound;
    [Range(0f, 1f)] public float pickupVolume = 1f;

    private bool picked = false;
    private float nextPickupTime = 0f;
    private bool processingPickup = false;

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
        if (processingPickup) return;
        if (!other.CompareTag("Player")) return;
        if (Time.time < nextPickupTime) return;
        if (picked) return;

        processingPickup = true;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null)
            health = other.GetComponentInParent<PlayerHealth>();

        if (health != null && !health.IsFullHealth)
        {
            int healAmount;
            if (TryGetHealAmount(out healAmount) && healAmount > 0)
            {
                bool healed = health.TryHeal(healAmount);
                if (healed)
                {
                    DungeonSpawnedObject dungeonMarker = GetComponent<DungeonSpawnedObject>();
                    if (dungeonMarker != null && DungeonManager.Instance != null)
                        DungeonManager.Instance.MarkDungeonObjectRemoved(dungeonMarker);

                    Collider2D col = GetComponent<Collider2D>();
                    if (col != null)
                        col.enabled = false;

                    gameObject.SetActive(false);
                    Destroy(gameObject);
                    return;
                }
            }
        }

        PlayerInventory inv = other.GetComponent<PlayerInventory>();
        if (inv == null)
            inv = other.GetComponentInParent<PlayerInventory>();
        if (inv == null)
        {
            processingPickup = false;
            return;
        }

        if (inv.TryAddItem(gameObject))
        {
            picked = true;
            PlayPickupSound();
        }

        processingPickup = false;
    }

    private bool TryGetHealAmount(out int amount)
    {
        amount = 0;

        HealPickupItem healPickup = GetComponent<HealPickupItem>();
        if (healPickup != null)
        {
            amount = healPickup.healAmount;
            return amount > 0;
        }

        BandagePickup bandagePickup = GetComponent<BandagePickup>();
        if (bandagePickup != null)
        {
            amount = bandagePickup.healAmount;
            return amount > 0;
        }

        return false;
    }

    private void PlayPickupSound()
    {
        if (pickupSound == null) return;
        AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupVolume);
    }

    public void OnDropped()
    {
        picked = false;
        processingPickup = false;
        nextPickupTime = Time.time + rePickupDelay;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = true;
        }
    }
}
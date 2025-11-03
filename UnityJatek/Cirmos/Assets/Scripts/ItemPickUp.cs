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
    public float rePickupDelay = 0.25f;   // dobás után ennyi ideig ne lehessen rögtön felvenni

    private bool picked = false;
    private float nextPickupTime = 0f;

    private void Reset()
    {
        var c = GetComponent<Collider2D>();
        if (c) c.isTrigger = true;
    }

    private void OnValidate()
    {
        var c = GetComponent<Collider2D>();
        if (c) c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time < nextPickupTime) return;  // várjunk kicsit dobás után
        if (picked) return;                      // már inventoryban van

        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null) inv = other.GetComponentInParent<PlayerInventory>();
        if (inv == null) return;

        if (inv.TryAddItem(gameObject))
        {
            picked = true; // inventoryba tettük → jelöljük felvettnek
        }
    }

    /// Meghívja a PlayerInventory, amikor kidobjuk.
    public void OnDropped()
    {
        picked = false;
        nextPickupTime = Time.time + rePickupDelay;

        var col = GetComponent<Collider2D>();
        if (col)
        {
            col.enabled = true;
            col.isTrigger = true;
        }
    }
}

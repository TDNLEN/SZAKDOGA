using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AmmoBox : MonoBehaviour
{
    public AmmoType ammoType = AmmoType.Handgun;
    public int amount = 8;          
    public bool autoPickup = true;   

    private void Reset()
    {
        var c = GetComponent<Collider2D>();
        if (c) c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!autoPickup) return;

        if (!other.CompareTag("Player")) return;

        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null) inv = other.GetComponentInParent<PlayerInventory>();
        if (inv == null) return;

        inv.AddAmmo(ammoType, amount);
        Destroy(gameObject);
    }

    
}

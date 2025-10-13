using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickUp : MonoBehaviour
{
    public Vector3 equipLocalPosition = new Vector3(0.25f, -0.05f, 0f);
    public Vector3 equipLocalRotation = new Vector3(0f, 0f, -20f);
    public Vector3 equipLocalScale = Vector3.one;

    private bool picked;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true; 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (picked) return;
        if (!other.CompareTag("Player")) return;

        var pc = other.GetComponentInParent<PlayerCombat>();
        
        picked = true;

        pc.PickUpItem(
            gameObject,
            equipLocalPosition,
            Quaternion.Euler(equipLocalRotation),
            equipLocalScale
        );
    }
}

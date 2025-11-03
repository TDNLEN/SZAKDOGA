using UnityEngine;

public class ZombieTouchDamage : MonoBehaviour
{
    public int dmg = 1;
    public float hitCD = 1.5f;
    public string playerTag = "Player";

    private float nextHitTime = 0f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if(Time.time <nextHitTime) return; 

        var health =other.GetComponent<PlayerHealth>();
        if(health == null) health= other.GetComponentInParent<PlayerHealth>();
        if (health == null) return;

        var zh = GetComponentInParent<ZombieHealth>();
        if (zh != null && IsZombieDead(zh)) return;

        health.TakeDamage(dmg);
        nextHitTime = Time.time + hitCD;

    }

    private bool IsZombieDead(ZombieHealth zh)
    {
        return false;
    }
}

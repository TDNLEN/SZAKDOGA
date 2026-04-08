using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public int damage = 1;
    public string targetTag = "Enemy";

    private HashSet<ZombieHealth> alreadyHit = new HashSet<ZombieHealth>();

    private void OnEnable()
    {
        alreadyHit.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(targetTag)) return;

        ZombieHealth health = collision.GetComponent<ZombieHealth>();
        if (health == null) return;

        if (alreadyHit.Contains(health)) return;

        health.TakeDamage(damage);
        alreadyHit.Add(health);
    }
}

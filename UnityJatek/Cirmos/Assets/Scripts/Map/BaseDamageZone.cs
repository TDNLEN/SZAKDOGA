using System.Collections.Generic;
using UnityEngine;

public class BaseDamageZone : MonoBehaviour
{
    [Header("Damage")]
    public int damagePerSecond = 50;
    public float tickInterval = 1f;

    [Header("Target")]
    public string enemyTag = "Enemy";
    public string playerTag = "Player";

    private readonly List<Collider2D> enemiesInside = new List<Collider2D>();
    private float timer = 0f;

    private PlayerHealth protectedPlayerHealth;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void Update()
    {
        if (enemiesInside.Count == 0) return;

        timer += Time.deltaTime;

        if (timer >= tickInterval)
        {
            timer = 0f;
            DamageAllInside();
        }
    }

    private void DamageAllInside()
    {
        for (int i = enemiesInside.Count - 1; i >= 0; i--)
        {
            Collider2D targetCol = enemiesInside[i];

            if (targetCol == null)
            {
                enemiesInside.RemoveAt(i);
                continue;
            }

            GameObject target = targetCol.gameObject;

            if (!target.CompareTag(enemyTag))
                continue;

            ZombieHealth zombieHealth = target.GetComponent<ZombieHealth>();
            if (zombieHealth == null)
                zombieHealth = target.GetComponentInParent<ZombieHealth>();

            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(damagePerSecond);
                continue;
            }

            Debug.LogWarning("A BaseDamageZone nem talált ismert health scriptet ezen az enemy objecten: " + target.name);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag))
        {
            if (!enemiesInside.Contains(other))
                enemiesInside.Add(other);

            return;
        }

        if (other.CompareTag(playerTag))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth == null)
                playerHealth = other.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                protectedPlayerHealth = playerHealth;
                protectedPlayerHealth.invulnerable = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (enemiesInside.Contains(other))
            enemiesInside.Remove(other);

        if (other.CompareTag(playerTag))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth == null)
                playerHealth = other.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.invulnerable = false;

                if (protectedPlayerHealth == playerHealth)
                    protectedPlayerHealth = null;
            }
        }
    }
}
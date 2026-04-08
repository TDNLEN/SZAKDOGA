using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrainDamage : MonoBehaviour
{
    public int damage = 999;
    public string enemyTag = "Enemy";

    private TrainController controller;

    private void Awake()
    {
        controller = GetComponentInParent<TrainController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryHit(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryHit(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryHit(other);
    }

    private void TryHit(Collider2D col)
    {
        if (controller == null) return;
        if (!controller.IsMoving) return;             
        if (!col.CompareTag(enemyTag)) return;

        var hp = col.GetComponent<ZombieHealth>();
        if (hp == null) hp = col.GetComponentInParent<ZombieHealth>();
        if (hp == null) return;

        hp.TakeDamage(damage, transform.position);
    }
}

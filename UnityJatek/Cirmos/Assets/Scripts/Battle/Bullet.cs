using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[DisallowMultipleComponent]
public class Bullet : MonoBehaviour
{
    private Vector2 dir;
    private float speed;
    private float maxDistance;
    private int damage;
    private Vector2 startPos;
    private string targetTag = "Enemy";

    public void Init(Vector2 direction, float speed, float maxDistance, int damage, string targetTag)
    {
        this.dir = direction.normalized;
        this.speed = speed;
        this.maxDistance = maxDistance;
        this.damage = damage;
        this.targetTag = string.IsNullOrEmpty(targetTag) ? "Enemy" : targetTag;
        this.startPos = transform.position;
    }

    private void Update()
    {
        transform.position += (Vector3)(dir * speed * Time.deltaTime);

        if (Vector2.Distance(startPos, transform.position) >= maxDistance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        var zh = other.GetComponent<ZombieHealth>();
        if (zh == null) zh = other.GetComponentInParent<ZombieHealth>();
        if (zh != null) zh.TakeDamage(damage);

        Destroy(gameObject);
    }
}

using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;
    public float maxDistance = 8f;

    private Vector2 direction;
    private Vector2 startPos;

    public void Init(Vector2 dir, float spd, int dmg, float range)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        maxDistance = range;
        startPos = transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        if (Vector2.Distance(startPos, transform.position) >= maxDistance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;

        var zh = other.GetComponent<ZombieHealth>();
        if (zh == null) zh = other.GetComponentInParent<ZombieHealth>();

        if (zh != null)
        {
            zh.TakeDamage(damage, transform.position);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}

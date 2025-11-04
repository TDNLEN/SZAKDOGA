using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;
    public float maxDistance = 8f;

    private Vector2 direction;
    private Vector2 startPos;

    // Ranged fegyver innen hívja
    public void Init(Vector2 dir, float spd, int dmg, float range)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        maxDistance = range;
        startPos = transform.position;

        // sprite fordítása a haladási irányba (opcionális, de jól néz ki)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Update()
    {
        // mozgás
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // táv végén töröljük
        if (Vector2.Distance(startPos, transform.position) >= maxDistance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ne a playert lõjük szét (ha enemy bullet, akkor majd mást csinál)
        if (other.CompareTag("Player")) return;

        // enemy sebzés
        var zh = other.GetComponent<ZombieHealth>();
        if (zh == null) zh = other.GetComponentInParent<ZombieHealth>();

        if (zh != null)
        {
            zh.TakeDamage(damage, transform.position);
            Destroy(gameObject);
            return;
        }

        // egyéb tárgyba csapódva is eltûnhet
        Destroy(gameObject);
    }
}

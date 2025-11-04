using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 1;
    public float speed = 10f;
    public float lifeTime = 3f;
    public string playerTag = "Player";

    private Vector2 direction;

    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
        // ha Rigidbody2D-vel akarod
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * speed;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // fallback, ha nincs RB
        if (GetComponent<Rigidbody2D>() == null)
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        var hp = other.GetComponent<PlayerHealth>();
        if (hp == null) hp = other.GetComponentInParent<PlayerHealth>();
        if (hp == null) return;

        hp.TakeDamage(damage);
        Destroy(gameObject);
    }
}

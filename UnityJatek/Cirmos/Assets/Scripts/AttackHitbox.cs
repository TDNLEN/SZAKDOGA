//using UnityEngine;

//public class AttackHitbox : MonoBehaviour
//{
//    public int damage = 1;
//    public string targetTag = "Enemy";
//    public bool destroyOnHit = false;

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (!other.CompareTag(targetTag)) return;

//        // Ha az enemy-nek van Health scriptje, hívd meg
//        var health = other.GetComponent<Health>();
//        if (health != null)
//        {
//            health.TakeDamage(damage);
//        }
//        else
//        {
//            Debug.Log("Találat: " + other.name);
//        }

//        if (destroyOnHit)
//            Destroy(gameObject);
//    }
//}

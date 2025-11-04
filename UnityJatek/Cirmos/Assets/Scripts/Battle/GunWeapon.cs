using UnityEngine;

[DisallowMultipleComponent]
public class GunWeapon : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;   // <- PREFAB, nem komponens!
    public Transform muzzle;          // ha üres, a fegyver pozícióját használjuk
    public float bulletSpeed = 10f;
    public float bulletRange = 8f;    // ennyit repül
    public int damage = 1;
    public float fireCooldown = 0.25f;

    private float nextShootTime = 0f;

    /// <summary>
    /// A világban lévõ célpont felé lõ (pl. egér pozíció).
    /// </summary>
    public bool TryShootTowards(Vector2 worldTarget)
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("GunWeapon: nincs bulletPrefab beállítva!");
            return false;
        }

        if (Time.time < nextShootTime)
            return false;

        nextShootTime = Time.time + fireCooldown;

        Vector3 spawnPos = muzzle != null ? muzzle.position : transform.position;
        Vector2 dir = ((Vector2)worldTarget - (Vector2)spawnPos).normalized;

        GameObject go = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        Bullet b = go.GetComponent<Bullet>();
        if (b != null)
        {
            b.Init(dir, bulletSpeed, damage, bulletRange);
        }
        else
        {
            Debug.LogWarning("GunWeapon: a bulletPrefab-on nincs Bullet script!");
        }

        return true;
    }
}

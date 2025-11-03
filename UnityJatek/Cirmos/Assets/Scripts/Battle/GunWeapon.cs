using UnityEngine;

[DisallowMultipleComponent]
public class GunWeapon : MonoBehaviour
{
    [Header("Shooting")]
    public Bullet bulletPrefab;
    public Transform muzzle;          // ha üres, a fegyver pozícióját használjuk
    public float bulletSpeed = 10f;
    public float bulletRange = 8f;    // ennyit repül
    public int damage = 1;
    public float fireCooldown = 0.25f;

    [Header("Targets")]
    public string targetTag = "Enemy";

    private float nextShootTime = 0f;

    
    public void TryShoot(Vector2 direction, Vector2 hitSource)
    {
        if (Time.time < nextShootTime) return;
        nextShootTime = Time.time + fireCooldown;

        var spawn = muzzle != null ? muzzle.position : transform.position;
        var b = Instantiate(bulletPrefab, spawn, Quaternion.identity);
        b.Init(direction, bulletSpeed, bulletRange, damage, targetTag);
    }
}

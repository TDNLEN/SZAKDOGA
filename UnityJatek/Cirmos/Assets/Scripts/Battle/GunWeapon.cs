using System.Collections;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class GunWeapon : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform muzzle;
    public float bulletSpeed = 10f;
    public float bulletRange = 8f;
    public int damage = 1;
    public float fireCooldown = 0.25f;

    [Header("Ammo")]
    public AmmoType ammoType = AmmoType.Handgun;
    public int magazineSize = 8;
    public int currentMag = 8;
    public float reloadTime = 2f;
    public TextMeshProUGUI ammoText;

    [Header("Shotgun settings")]
    public bool isShotgun = false;
    public int pelletsPerShot = 5;
    public float spreadAngle = 15f;

    [Header("Rifle burst settings")]
    public bool isBurstRifle = false;
    public int burstCount = 3;
    public float burstInterval = 0.08f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shotSound;
    [Range(0f, 1f)] public float shotVolume = 1f;

    private float nextShootTime = 0f;
    private bool isEquipped = false;
    private bool isReloading = false;
    private Coroutine reloadRoutine;

    private PlayerInventory ownerInventory;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        UpdateAmmoUI();
    }

    public void SetOwnerInventory(PlayerInventory inv)
    {
        ownerInventory = inv;
        UpdateAmmoUI();
    }

    public void OnEquip()
    {
        isEquipped = true;
        UpdateAmmoUI();
    }

    public void OnUnequip()
    {
        isEquipped = false;

        if (ammoText != null)
            ammoText.gameObject.SetActive(false);
    }

    public bool TryShootTowards(Vector2 worldTarget)
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("GunWeapon: nincs bulletPrefab beállítva!");
            return false;
        }

        if (isReloading)
            return false;

        if (Time.time < nextShootTime)
            return false;

        if (currentMag <= 0)
        {
            TryStartReload();
            return false;
        }

        nextShootTime = Time.time + fireCooldown;

        Vector3 spawnPos = muzzle != null ? muzzle.position : transform.position;
        Vector2 baseDir = ((Vector2)worldTarget - (Vector2)spawnPos).normalized;

        if (isBurstRifle)
        {
            StartCoroutine(BurstRoutine(spawnPos, baseDir));
        }
        else
        {
            currentMag--;
            UpdateAmmoUI();

            PlayShotSound();

            if (isShotgun)
            {
                float half = spreadAngle * 0.5f;
                for (int i = 0; i < pelletsPerShot; i++)
                {
                    float angleOffset = Random.Range(-half, half);
                    Vector2 pelletDir = Quaternion.Euler(0, 0, angleOffset) * baseDir;
                    SpawnBullet(spawnPos, pelletDir);
                }
            }
            else
            {
                SpawnBullet(spawnPos, baseDir);
            }

            if (currentMag <= 0)
                TryStartReload();
        }

        return true;
    }

    private IEnumerator BurstRoutine(Vector3 spawnPos, Vector2 dir)
    {
        int shots = Mathf.Min(burstCount, currentMag);

        for (int i = 0; i < shots; i++)
        {
            currentMag--;
            UpdateAmmoUI();

            PlayShotSound();
            SpawnBullet(spawnPos, dir);

            if (currentMag <= 0)
            {
                TryStartReload();
                yield break;
            }

            if (i < shots - 1)
                yield return new WaitForSeconds(burstInterval);
        }
    }

    private void SpawnBullet(Vector3 spawnPos, Vector2 dir)
    {
        GameObject go = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        Bullet b = go.GetComponent<Bullet>();
        if (b != null)
            b.Init(dir, bulletSpeed, damage, bulletRange);
        else
            Debug.LogWarning("GunWeapon: a bulletPrefab-on nincs Bullet script!");
    }

    private void PlayShotSound()
    {
        if (audioSource == null || shotSound == null)
            return;

        audioSource.PlayOneShot(shotSound, shotVolume);
    }

    private void TryStartReload()
    {
        if (isReloading) return;
        if (ownerInventory == null) return;

        int reserve = ownerInventory.GetReserveAmmo(ammoType);
        if (reserve <= 0) return;

        reloadRoutine = StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);

        if (ownerInventory == null)
        {
            isReloading = false;
            yield break;
        }

        int needed = magazineSize - currentMag;
        if (needed > 0)
        {
            int taken = ownerInventory.ConsumeAmmo(ammoType, needed);
            currentMag += taken;
        }

        isReloading = false;
        UpdateAmmoUI();
        reloadRoutine = null;
    }

    private void UpdateAmmoUI()
    {
        if (ammoText == null) return;

        if (!isEquipped)
        {
            ammoText.gameObject.SetActive(false);
            return;
        }

        int reserve = ownerInventory != null
            ? ownerInventory.GetReserveAmmo(ammoType)
            : 0;

        ammoText.gameObject.SetActive(true);
        ammoText.text = $"{currentMag}/{reserve}";
    }

    public void RefreshAmmoUI() => UpdateAmmoUI();
}
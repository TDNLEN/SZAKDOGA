using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class GunWeapon : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;      // PREFAB!
    public Transform muzzle;             // ha üres, a fegyver pozícióját használjuk
    public float bulletSpeed = 10f;
    public float bulletRange = 8f;
    public int damage = 1;
    public float fireCooldown = 0.25f;

    [Header("Ammo")]
    public int magazineSize = 8;         // tár méret (első szám)
    public int currentMag = 8;           // jelenlegi töltény a tárban
    public int reserveAmmo = 8;          // tartalék (második szám)
    public float reloadTime = 2f;        // 2 mp reload
    public TextMeshProUGUI ammoText;     // UI: pl. "8 / 8"

    private float nextShootTime = 0f;
    private bool isEquipped = false;
    private bool isReloading = false;
    private Coroutine reloadRoutine;

    private void OnEnable()
    {
        UpdateAmmoUI();
    }

    // player kézbe veszi
    public void OnEquip()
    {
        isEquipped = true;
        UpdateAmmoUI();
    }

    // player elrakja
    public void OnUnequip()
    {
        isEquipped = false;

        if (ammoText != null)
            ammoText.gameObject.SetActive(false);
    }

    /// <summary>
    /// A világban lévő célpont felé lő (pl. egér pozíció).
    /// </summary>
    public bool TryShootTowards(Vector2 worldTarget)
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("GunWeapon: nincs bulletPrefab beállítva!");
            return false;
        }

        // ha épp tölt, nem lövünk
        if (isReloading)
            return false;

        // cooldown
        if (Time.time < nextShootTime)
            return false;

        // üres tár → automata reload, ha van tartalék
        if (currentMag <= 0)
        {
            TryStartReload();
            return false;
        }

        nextShootTime = Time.time + fireCooldown;

        // lövés → -1 a tárból
        currentMag--;
        UpdateAmmoUI();

        // golyó spawn
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

        // ha ezzel lőttük ki az utolsó golyót, indulhat az auto reload
        if (currentMag <= 0)
        {
            TryStartReload();
        }

        return true;
    }

    // csak akkor induljon reload, ha van tartalék
    private void TryStartReload()
    {
        if (isReloading) return;
        if (reserveAmmo <= 0) return; // nincs tartalék → nincs reload

        reloadRoutine = StartCoroutine(ReloadCoroutine());
    }

    private System.Collections.IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        // ide rakhatsz reload anim / hangot is

        yield return new WaitForSeconds(reloadTime);

        // mennyit tudunk a tárba tenni?
        int needed = magazineSize - currentMag;
        int toLoad = Mathf.Min(needed, reserveAmmo);

        currentMag += toLoad;
        reserveAmmo -= toLoad;

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

        ammoText.gameObject.SetActive(true);
        ammoText.text = $"{currentMag}/{reserveAmmo}";
    }

    // ha később akarsz “instant full reload” powerupot, ezt is használhatod:
    public void RefillAllAmmo(int magAmount, int reserveAmount)
    {
        magazineSize = magAmount;
        currentMag = magAmount;
        reserveAmmo = reserveAmount;
        UpdateAmmoUI();
    }

    // a GunWeapon osztályodon belül (add előző kódhoz)
    public void AddReserve(int amount)
    {
        if (amount <= 0) return;

        reserveAmmo += amount;
        // opcionálisan clamp: ha akarsz maxot (pl. 999)
        // reserveAmmo = Mathf.Min(reserveAmmo, someMax);

        UpdateAmmoUI();
    }

}

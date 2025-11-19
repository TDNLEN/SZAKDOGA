using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Setup")]
    public int slotCount = 5;
    public HotbarUI hotbar;
    public PlayerCombat playerCombat;
    public Transform swordHolder;

    [Header("Controls")]
    public KeyCode dropKey = KeyCode.Q;

    private GameObject[] items;
    private int selected = 0;

    [Header("Stored Ammo (shared, not in inventory)")]
    public int storedHandgunAmmo = 0;
    public int storedShotgunAmmo = 0;
    public int storedRifleAmmo = 0;    // <<< ÚJ

    // ---------- lifecycle ----------

    private void Awake()
    {
        items = new GameObject[slotCount];
    }

    private void Start()
    {
        if (hotbar != null && hotbar.slotBGs != null && hotbar.slotBGs.Length > 0)
            hotbar.Select(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Select(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Select(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Select(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) Select(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) Select(4);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0.05f) SelectNext(+1);
        else if (scroll < -0.05f) SelectNext(-1);

        if (Input.GetKeyDown(dropKey)) DropSelected();
    }

    // ---------- slot váltás ----------

    public void Select(int index)
    {
        index = Mathf.Clamp(index, 0, slotCount - 1);

        UnequipCurrent();

        selected = index;
        if (hotbar) hotbar.Select(index);

        EquipFromSlot(selected);
    }

    public void SelectNext(int dir)
    {
        int i = (selected + dir) % slotCount;
        if (i < 0) i += slotCount;
        Select(i);
    }

    public void SelectSlot(int i) => Select(i);

    // ---------- pickup / drop ----------

    public bool TryAddItem(GameObject worldItem)
    {
        int free = FindFreeSlot();
        if (free < 0) return false;

        Sprite icon = null;
        var sr = worldItem.GetComponent<SpriteRenderer>();
        if (sr) icon = sr.sprite;
        if (hotbar) hotbar.SetIcon(free, icon);

        items[free] = worldItem;
        var col = worldItem.GetComponent<Collider2D>();
        if (col) col.enabled = false;
        worldItem.SetActive(false);

        if (free == selected)
            EquipFromSlot(selected);

        return true;
    }

    private int FindFreeSlot()
    {
        for (int i = 0; i < items.Length; i++)
            if (items[i] == null) return i;
        return -1;
    }

    private void EquipFromSlot(int i)
    {
        var item = items[i];
        if (item == null) return;

        item.SetActive(true);

        var ipu = item.GetComponent<ItemPickUp>();
        Vector3 lp = ipu ? ipu.equipLocalPosition : Vector3.zero;
        Quaternion lr = ipu ? Quaternion.Euler(ipu.equipLocalRotation) : Quaternion.identity;
        Vector3 ls = ipu ? ipu.equipLocalScale : Vector3.one;

        if (playerCombat)
        {
            playerCombat.PickUpItem(item, lp, lr, ls);

            var gun = item.GetComponent<GunWeapon>() ?? item.GetComponentInChildren<GunWeapon>();
            if (gun != null)
            {
                gun.SetOwnerInventory(this);
                gun.OnEquip();
            }
        }
        else if (swordHolder)
        {
            item.transform.SetParent(swordHolder, false);
            item.transform.localPosition = lp;
            item.transform.localRotation = lr;
            item.transform.localScale = ls;
        }
    }

    private void UnequipCurrent()
    {
        if (playerCombat != null)
            playerCombat.UnequipCurrent();
    }

    private void DropSelected()
    {
        var item = items[selected];
        if (item == null) return;

        if (playerCombat && playerCombat.equippedSword == item)
            UnequipCurrent();

        item.transform.SetParent(null);
        item.transform.position = transform.position + Vector3.right * 0.6f;

        var col = item.GetComponent<Collider2D>();
        if (col)
        {
            col.enabled = true;
            col.isTrigger = true;
        }

        var ipu = item.GetComponent<ItemPickUp>();
        if (ipu) ipu.OnDropped();

        item.SetActive(true);

        items[selected] = null;
        if (hotbar) hotbar.ClearIcon(selected);
    }

    // ---------- Train fuel ----------

    public bool TryConsumeSelectedFuelItem(out int fuelGained)
    {
        fuelGained = 0;

        var item = items[selected];
        if (item == null) return false;

        var fuel = item.GetComponent<FuelItem>();
        if (fuel == null) return false;

        fuelGained = fuel.fuelAmount;

        items[selected] = null;
        if (hotbar) hotbar.ClearIcon(selected);

        Destroy(item);
        return true;
    }

    public bool HasFuelInSelectedSlot()
    {
        var item = items[selected];
        if (item == null) return false;

        return item.GetComponent<FuelItem>() != null;
    }

    // ---------- Shared ammo pool ----------

    public void AddAmmo(AmmoType type, int amount)
    {
        if (amount <= 0) return;

        switch (type)
        {
            case AmmoType.Handgun:
                storedHandgunAmmo += amount;
                break;

            case AmmoType.Shotgun:
                storedShotgunAmmo += amount;
                break;

            case AmmoType.Rifle:
                storedRifleAmmo += amount;
                break;
        }

        RefreshEquippedGunUI();
    }

    public int GetReserveAmmo(AmmoType type)
    {
        return type switch
        {
            AmmoType.Handgun => storedHandgunAmmo,
            AmmoType.Shotgun => storedShotgunAmmo,
            AmmoType.Rifle => storedRifleAmmo,
            _ => 0
        };
    }

    public int ConsumeAmmo(AmmoType type, int amount)
    {
        if (amount <= 0) return 0;

        int taken = 0;

        switch (type)
        {
            case AmmoType.Handgun:
                taken = Mathf.Min(amount, storedHandgunAmmo);
                storedHandgunAmmo -= taken;
                break;

            case AmmoType.Shotgun:
                taken = Mathf.Min(amount, storedShotgunAmmo);
                storedShotgunAmmo -= taken;
                break;

            case AmmoType.Rifle:
                taken = Mathf.Min(amount, storedRifleAmmo);
                storedRifleAmmo -= taken;
                break;
        }

        if (taken > 0)
            RefreshEquippedGunUI();

        return taken;
    }

    private void RefreshEquippedGunUI()
    {
        if (playerCombat == null || playerCombat.equippedSword == null) return;

        var gun = playerCombat.equippedSword.GetComponent<GunWeapon>() ??
                  playerCombat.equippedSword.GetComponentInChildren<GunWeapon>();

        if (gun != null)
            gun.RefreshAmmoUI();
    }
}

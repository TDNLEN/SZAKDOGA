using UnityEngine;

[System.Serializable]
public class ShopItem
{
    public string id;              // csak név / azonosító (nem kötelező semmire)
    public GameObject itemPrefab;  // FEgyver / tárgy prefab (amin van ItemPickUp!)
    public int price = 10;         // ára coinban
}

public class WeaponShop : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;             // Player Transform
    public PlayerWallet playerWallet;    // PlayerWallet script (coins)
    public PlayerInventory playerInventory; // PlayerInventory script

    [Header("Interact")]
    public float interactRadius = 10f;   // ennyi távolságból nyíljon G-vel
    public GameObject gPrompt;           // "G" ikon a player felett
    public GameObject shopUI;            // a teljes shop panel (Canvas-en egy Panel)

    [Header("Drop")]
    public Transform dropPoint;          // ha nincs hely az inventoryban, ide dobjuk (pl. bolt elé)

    [Header("Items in shop")]
    public ShopItem[] items;             // itt állítod be az elérhető fegyvereket

    private bool isOpen = false;

    private void Start()
    {
        if (shopUI != null)
            shopUI.SetActive(false);

        if (gPrompt != null)
            gPrompt.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(player.position, transform.position);
        bool inRange = dist <= interactRadius;

        if (!isOpen)
        {
            // csak akkor mutassuk a G-t, ha közel vagyunk
            if (gPrompt != null)
                gPrompt.SetActive(inRange);

            // G → shop megnyitása
            if (inRange && Input.GetKeyDown(KeyCode.G))
            {
                OpenShop();
            }
        }
        else
        {
            // ha nyitva a shop:
            // G-vel vagy ha nagyon eltávolodunk → zárjuk be
            if (Input.GetKeyDown(KeyCode.G) || !inRange)
            {
                CloseShop();
            }
        }
    }

    private void OpenShop()
    {
        isOpen = true;
        if (shopUI != null)
            shopUI.SetActive(true);

        if (gPrompt != null)
            gPrompt.SetActive(false);

        // ha akarod, itt letilthatod a player mozgást is (TopDownMover.enabled = false)
    }

    public void CloseShop()
    {
        isOpen = false;
        if (shopUI != null)
            shopUI.SetActive(false);
        // G prompt-ot majd az Update kezeli újra
    }

    /// <summary>
    /// UI gombok hívják: paraméter az items tömb indexe (0,1,2,...).
    /// </summary>
    public void BuyItem(int index)
    {
        if (!isOpen) return;
        if (items == null || index < 0 || index >= items.Length) return;
        if (playerWallet == null || playerInventory == null) return;

        ShopItem si = items[index];
        if (si.itemPrefab == null)
        {
            Debug.LogWarning("ShopItem prefab nincs beállítva!");
            return;
        }

        // elég pénz?
        if (!playerWallet.TrySpend(si.price))
        {
            Debug.Log("Nincs elég coin a vásárláshoz.");
            return;
        }

        // hol spawnoljon a tárgy (ha nem fér inventoryba)?
        Vector3 spawnPos = dropPoint != null
            ? dropPoint.position
            : player.position + Vector3.right * 0.6f;

        // Létrehozzuk a világban (mindenképp)
        GameObject itemInstance = Instantiate(si.itemPrefab, spawnPos, Quaternion.identity);

        // megpróbáljuk inventoryba rakni
        bool added = playerInventory.TryAddItem(itemInstance);

        if (!added)
        {
            // NINCS hely az inventoryban → maradjon a földön a shop előtt
            var col = itemInstance.GetComponent<Collider2D>();
            if (col)
            {
                col.enabled = true;
                col.isTrigger = true;
            }

            var ipu = itemInstance.GetComponent<ItemPickUp>();
            if (ipu)
                ipu.OnDropped();   // hogy azonnal felvehető legyen kis delay után
        }

        Debug.Log($"Megvetted: {si.id} ({si.price} coin)");
    }
}

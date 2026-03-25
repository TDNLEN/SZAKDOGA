using UnityEngine;

public class FuelShop : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public PlayerWallet playerWallet;
    public PlayerInventory playerInventory;

    [Header("Interact")]
    public float interactRadius = 5f;
    public GameObject gPrompt;
    public GameObject shopUI;

    [Header("Drop")]
    public Transform dropPoint;

    [Header("Items")]
    public ShopItem[] items;

    private bool isOpen = false;

    private void Awake()
    {
        AutoAssignReferences();
    }

    private void Start()
    {
        if (shopUI != null) shopUI.SetActive(false);
        if (gPrompt != null) gPrompt.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(player.position, transform.position);
        bool inRange = dist <= interactRadius;

        if (!isOpen)
        {
            if (gPrompt != null) gPrompt.SetActive(inRange);

            if (inRange && Input.GetKeyDown(KeyCode.G))
                OpenShop();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.G) || !inRange)
                CloseShop();
        }
    }

    private void AutoAssignReferences()
    {
        if (player == null)
        {
            GameObject playerObj = FindSceneObjectByExactName("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (playerWallet == null)
            playerWallet = Object.FindFirstObjectByType<PlayerWallet>();

        if (playerInventory == null)
            playerInventory = Object.FindFirstObjectByType<PlayerInventory>();

        if (gPrompt == null)
            gPrompt = FindChildUnderRootFuzzy("Player", "gpromptfuel");

        if (shopUI == null)
            shopUI = FindChildUnderRootFuzzy("PlayerUI", "fuelshoppanel");

        if (dropPoint == null)
        {
            Transform t = FindDeepChild(transform, "DropPoint");
            if (t == null) t = FindDeepChild(transform, "Drop");
            if (t == null) t = FindDeepChild(transform, "SpawnPoint");
            if (t != null) dropPoint = t;
        }
    }

    private GameObject FindSceneObjectByExactName(string objectName)
    {
        Transform[] all = Resources.FindObjectsOfTypeAll<Transform>();

        foreach (Transform t in all)
        {
            if (t == null) continue;
            if (t.name != objectName) continue;
            if (t.hideFlags != HideFlags.None) continue;
            if (!t.gameObject.scene.IsValid()) continue;
            return t.gameObject;
        }

        return null;
    }

    private GameObject FindChildUnderRootFuzzy(string rootName, string wantedNormalizedName)
    {
        GameObject rootObj = FindSceneObjectByExactName(rootName);
        if (rootObj == null) return null;

        Transform[] children = rootObj.GetComponentsInChildren<Transform>(true);

        foreach (Transform t in children)
        {
            string n = NormalizeName(t.name);
            if (n.Contains(wantedNormalizedName))
                return t.gameObject;
        }

        return null;
    }

    private string NormalizeName(string s)
    {
        s = s.ToLower();
        s = s.Replace(" ", "");
        s = s.Replace("_", "");
        s = s.Replace("-", "");
        s = s.Replace("(", "");
        s = s.Replace(")", "");
        s = s.Replace(".", "");
        return s;
    }

    private Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == childName)
                return child;
        }

        return null;
    }

    private void OpenShop()
    {
        isOpen = true;

        if (shopUI != null)
        {
            ItemShopPanel panel = shopUI.GetComponent<ItemShopPanel>();
            if (panel != null)
                panel.SetFuelShop(this);

            shopUI.SetActive(true);
        }

        if (gPrompt != null)
            gPrompt.SetActive(false);
    }

    public void CloseShop()
    {
        isOpen = false;

        if (shopUI != null)
            shopUI.SetActive(false);
    }

    public void BuyItem(int index)
    {
        Debug.Log("[FuelShop] BuyItem ezen fut: " + gameObject.name, this);

        if (!isOpen || index < 0 || index >= items.Length) return;
        if (playerWallet == null || playerInventory == null) return;

        ShopItem si = items[index];
        if (si.itemPrefab == null) return;

        if (!playerWallet.TrySpend(si.price))
        {
            Debug.Log("Nincs elég coin a vásárláshoz.");
            return;
        }

        Vector3 spawnPos = dropPoint != null
            ? dropPoint.position
            : player.position + Vector3.right * 0.6f;

        GameObject newObj = Instantiate(si.itemPrefab, spawnPos, Quaternion.identity);

        bool added = playerInventory.TryAddItem(newObj);

        if (!added)
        {
            Collider2D col = newObj.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = true;
                col.isTrigger = true;
            }

            newObj.GetComponent<ItemPickUp>()?.OnDropped();
        }

        Debug.Log($"[FuelShop] Megvetted: {si.id} ({si.price} coin)");
    }
}
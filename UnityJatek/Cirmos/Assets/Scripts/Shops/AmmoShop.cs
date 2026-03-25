using UnityEngine;

public class AmmoShop : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public PlayerWallet playerWallet;
    public PlayerInventory playerInventory;

    [Header("Interact")]
    public float interactRadius = 5f;
    public GameObject gPrompt;
    public GameObject shopUI;

    [Header("Ammo Products")]
    public AmmoShopItem[] items;

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
            gPrompt = FindChildUnderRootFuzzy("Player", "gpromptammo");

        if (shopUI == null)
            shopUI = FindChildUnderRootFuzzy("PlayerUI", "ammoshoppanel");
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

    private void OpenShop()
    {
        isOpen = true;

        if (shopUI != null)
        {
            AmmoShopPanelController panel = shopUI.GetComponent<AmmoShopPanelController>();
            if (panel != null)
                panel.SetAmmoShop(this);

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

    public void BuyAmmo(int index)
    {
        Debug.Log("[AmmoShop] BuyAmmo ezen fut: " + gameObject.name, this);

        if (!isOpen || index < 0 || index >= items.Length) return;
        if (playerWallet == null || playerInventory == null) return;

        AmmoShopItem si = items[index];

        if (!playerWallet.TrySpend(si.price))
        {
            Debug.Log("Nincs elég coin a vásárláshoz.");
            return;
        }

        playerInventory.AddAmmo(si.ammoType, si.amount);
        Debug.Log($"[AmmoShop] Megvetted: {si.id} +{si.amount} ammo ({si.price} coin)");
    }
}
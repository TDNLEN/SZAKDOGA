using UnityEngine;

public class WeaponShop : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public PlayerWallet playerWallet;
    public PlayerInventory playerInventory;

    [Header("Interact")]
    public float interactRadius = 10f;
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
            if (gPrompt != null)
                gPrompt.SetActive(inRange);

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
            GameObject playerObj = FindSceneObjectByName("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (playerWallet == null)
            playerWallet = Object.FindFirstObjectByType<PlayerWallet>();

        if (playerInventory == null)
            playerInventory = Object.FindFirstObjectByType<PlayerInventory>();

        if (gPrompt == null)
            gPrompt = FindSceneObjectByName("G_Prompt_weapon");

        if (shopUI == null)
            shopUI = FindSceneObjectByName("WeaponShopPanel");

        if (dropPoint == null)
        {
            Transform t = FindDeepChild(transform, "DropPoint");
            if (t == null) t = FindDeepChild(transform, "Drop");
            if (t == null) t = FindDeepChild(transform, "SpawnPoint");
            if (t != null) dropPoint = t;
        }

        Debug.Log(
            $"[WeaponShop] refs -> player:{player != null}, wallet:{playerWallet != null}, inventory:{playerInventory != null}, gPrompt:{gPrompt != null}, shopUI:{shopUI != null}, dropPoint:{dropPoint != null}",
            this
        );
    }

    private GameObject FindSceneObjectByName(string objectName)
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
            shopUI.SetActive(true);

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
        if (!isOpen || index < 0 || index >= items.Length) return;
        if (playerWallet == null || playerInventory == null) return;

        ShopItem si = items[index];
        if (si.itemPrefab == null) return;

        if (!playerWallet.TrySpend(si.price))
        {
            Debug.Log("Nincs elég coin.");
            return;
        }

        Vector3 spawnPos = dropPoint != null
            ? dropPoint.position
            : player.position + Vector3.right * 0.6f;

        GameObject obj = Instantiate(si.itemPrefab, spawnPos, Quaternion.identity);
        bool added = playerInventory.TryAddItem(obj);

        if (!added)
        {
            Collider2D col = obj.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = true;
                col.isTrigger = true;
            }

            obj.GetComponent<ItemPickUp>()?.OnDropped();
        }

        Debug.Log($"[WeaponShop] Megvetted: {si.id} ({si.price} coin)");
    }
}
using UnityEngine;

public class HealShop : MonoBehaviour
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

    private void Start()
    {
        shopUI?.SetActive(false);
        gPrompt?.SetActive(false);
    }

    private void Update()
    {
        if (!player) return;

        float dist = Vector2.Distance(player.position, transform.position);
        bool inRange = dist <= interactRadius;

        if (!isOpen)
        {
            gPrompt?.SetActive(inRange);

            if (inRange && Input.GetKeyDown(KeyCode.G))
                OpenShop();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.G) || !inRange)
                CloseShop();
        }
    }

    private void OpenShop()
    {
        isOpen = true;
        shopUI?.SetActive(true);
        gPrompt?.SetActive(false);
    }

    public void CloseShop()
    {
        isOpen = false;
        shopUI?.SetActive(false);
    }

    public void BuyItem(int index)
    {
        if (!isOpen || index < 0 || index >= items.Length) return;
        if (!playerWallet || !playerInventory) return;

        var si = items[index];
        if (!si.itemPrefab) return;

        if (!playerWallet.TrySpend(si.price))
        {
            Debug.Log("Nincs elég coin a vásárláshoz.");
            return;
        }

        Vector3 spawnPos =
            dropPoint ? dropPoint.position : player.position + Vector3.right * 0.6f;

        var newObj = Instantiate(si.itemPrefab, spawnPos, Quaternion.identity);

        bool added = playerInventory.TryAddItem(newObj);

        if (!added)
        {
            var col = newObj.GetComponent<Collider2D>();
            if (col)
            {
                col.enabled = true;
                col.isTrigger = true;
            }

            newObj.GetComponent<ItemPickUp>()?.OnDropped();
        }

        Debug.Log($"[HealShop] Megvetted: {si.id} ({si.price} coin)");
    }
}

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

    public void BuyAmmo(int index)
    {
        if (!isOpen || index < 0 || index >= items.Length) return;
        if (!playerWallet || !playerInventory) return;

        var si = items[index];

        if (!playerWallet.TrySpend(si.price))
        {
            Debug.Log("Nincs elég coin a vásárláshoz.");
            return;
        }

        playerInventory.AddAmmo(si.ammoType, si.amount);
        Debug.Log($"Megvetted: {si.id} +{si.amount} ammo ({si.price} coin)");
    }
}

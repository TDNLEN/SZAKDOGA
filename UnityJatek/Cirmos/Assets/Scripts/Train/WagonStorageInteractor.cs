using UnityEngine;

public class WagonStorageInteractor : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public Transform interactPoint;
    public GameObject ePrompt;
    public GameObject storageUI;
    public WagonStorage wagonStorage;
    public PlayerInventory playerInventory;

    [Header("Settings")]
    public float interactRadius = 3f;
    public KeyCode interactKey = KeyCode.E;

    private bool isOpen = false;

    private void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (wagonStorage == null)
            wagonStorage = GetComponent<WagonStorage>();

        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (interactPoint == null)
            interactPoint = transform;
    }

    private void Start()
    {
        if (ePrompt != null)
            ePrompt.SetActive(false);

        if (storageUI != null)
            storageUI.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(player.position, interactPoint.position);
        bool inRange = dist <= interactRadius;

        if (!isOpen)
        {
            if (ePrompt != null)
                ePrompt.SetActive(inRange);

            if (inRange && Input.GetKeyDown(interactKey))
                OpenStorage();
        }
        else
        {
            if (Input.GetKeyDown(interactKey) || !inRange)
                CloseStorage();
        }
    }

    public void OpenStorage()
    {
        isOpen = true;

        if (ePrompt != null)
            ePrompt.SetActive(false);

        if (storageUI != null)
        {
            WagonStorageUI ui = storageUI.GetComponent<WagonStorageUI>();
            if (ui != null)
                ui.SetCurrentStorage(this);

            storageUI.SetActive(true);
        }
    }

    public void CloseStorage()
    {
        isOpen = false;

        if (storageUI != null)
            storageUI.SetActive(false);
    }

    public WagonStorage GetStorage()
    {
        return wagonStorage;
    }

    public PlayerInventory GetPlayerInventory()
    {
        return playerInventory;
    }
}
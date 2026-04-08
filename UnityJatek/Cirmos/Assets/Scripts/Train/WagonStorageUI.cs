using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WagonStorageUI : MonoBehaviour
{
    [System.Serializable]
    public class WagonSlotUI
    {
        public Image icon;
        public TMP_Text amountText;
        public Button button;
    }

    [Header("Slots")]
    public WagonSlotUI[] slots;

    [Header("Refs")]
    public WagonStorageInteractor currentInteractor;

    private void OnEnable()
    {
        Refresh();
    }

    private void Update()
    {
        if (currentInteractor == null) return;


        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverAnySlot())
            {
                PutSelectedPlayerItemIntoFirstEmptySlot();
            }
        }
    }

    public void SetCurrentStorage(WagonStorageInteractor interactor)
    {
        currentInteractor = interactor;
        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].icon != null)
            {
                slots[i].icon.sprite = null;
                slots[i].icon.enabled = false;
            }

            if (slots[i].amountText != null)
                slots[i].amountText.text = "";

            int capturedIndex = i;

            if (slots[i].button != null)
            {
                slots[i].button.onClick.RemoveAllListeners();
                slots[i].button.onClick.AddListener(() => TakeItemFromWagon(capturedIndex));
            }
        }

        if (currentInteractor == null) return;

        WagonStorage storage = currentInteractor.GetStorage();
        if (storage == null || storage.items == null) return;

        for (int i = 0; i < storage.items.Length && i < slots.Length; i++)
        {
            GameObject item = storage.items[i] != null ? storage.items[i].itemObject : null;
            if (item == null) continue;

            SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null && slots[i].icon != null)
            {
                slots[i].icon.sprite = sr.sprite;
                slots[i].icon.enabled = true;
            }

            if (slots[i].amountText != null)
                slots[i].amountText.gameObject.SetActive(false);

        }
    }

    private bool IsPointerOverAnySlot()
    {
        Vector2 mousePos = Input.mousePosition;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].button == null) continue;

            RectTransform rt = slots[i].button.GetComponent<RectTransform>();
            if (rt == null) continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos, null))
                return true;
        }

        return false;
    }

    private void PutSelectedPlayerItemIntoFirstEmptySlot()
    {
        if (currentInteractor == null) return;

        WagonStorage storage = currentInteractor.GetStorage();
        PlayerInventory playerInventory = currentInteractor.GetPlayerInventory();

        if (storage == null || playerInventory == null) return;

        int emptySlot = -1;
        for (int i = 0; i < storage.maxSlots; i++)
        {
            if (storage.IsSlotEmpty(i))
            {
                emptySlot = i;
                break;
            }
        }

        if (emptySlot < 0)
        {
            Debug.Log("A wagon inventory tele van.");
            return;
        }

        GameObject selectedItem = playerInventory.GetSelectedItem();
        if (selectedItem == null)
        {
            Debug.Log("Nincs item a kiválasztott player slotban.");
            return;
        }

        GameObject removedItem = playerInventory.RemoveSelectedItemFromInventory();
        if (removedItem == null)
            return;

        bool ok = storage.TryAddItemToSlot(emptySlot, removedItem);
        if (!ok)
        {
            playerInventory.TryAddStoredItem(removedItem);
            Debug.Log("Nem sikerült a tárgyat a wagonba tenni.");
            return;
        }

        removedItem.SetActive(false);

        Debug.Log("Tárgy betéve a wagonba: " + removedItem.name);
        Refresh();
    }

    private void TakeItemFromWagon(int index)
    {
        if (currentInteractor == null) return;

        WagonStorage storage = currentInteractor.GetStorage();
        PlayerInventory playerInventory = currentInteractor.GetPlayerInventory();

        if (storage == null || playerInventory == null) return;

        GameObject itemObject;
        bool ok = storage.TryGetItemAt(index, out itemObject);
        if (!ok || itemObject == null)
            return;

        bool added = playerInventory.TryAddStoredItem(itemObject);
        if (!added)
        {
            Debug.Log("Nincs szabad hely a player inventoryban.");
            return;
        }

        storage.RemoveItemAt(index);

        Debug.Log("Kivettél a wagonból: " + itemObject.name);
        Refresh();
    }
}
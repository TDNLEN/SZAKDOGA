using UnityEngine;

public class WagonStorage : MonoBehaviour
{
    [Header("Storage")]
    public int maxSlots = 6;

    [System.Serializable]
    public class StoredItem
    {
        public GameObject itemObject;
    }

    public StoredItem[] items;

    private void Awake()
    {
        if (items == null || items.Length != maxSlots)
            items = new StoredItem[maxSlots];
    }

    public bool IsValidIndex(int index)
    {
        return index >= 0 && index < maxSlots;
    }

    public bool IsSlotEmpty(int index)
    {
        if (!IsValidIndex(index)) return false;
        return items[index] == null || items[index].itemObject == null;
    }

    public bool TryAddItemToSlot(int index, GameObject itemObject)
    {
        if (!IsValidIndex(index)) return false;
        if (itemObject == null) return false;
        if (!IsSlotEmpty(index)) return false;

        StoredItem entry = new StoredItem();
        entry.itemObject = itemObject;
        items[index] = entry;
        return true;
    }

    public bool TryGetItemAt(int index, out GameObject itemObject)
    {
        itemObject = null;

        if (!IsValidIndex(index))
            return false;

        if (items[index] == null || items[index].itemObject == null)
            return false;

        itemObject = items[index].itemObject;
        return true;
    }

    public bool RemoveItemAt(int index)
    {
        if (!IsValidIndex(index))
            return false;

        items[index] = null;
        return true;
    }
}
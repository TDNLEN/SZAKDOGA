using NUnit.Framework;
using UnityEngine;

public class WagonStorageTests
{
    private GameObject storageObject;
    private WagonStorage storage;

    [SetUp]
    public void SetUp()
    {
        storageObject = new GameObject("WagonStorage");
        storage = storageObject.AddComponent<WagonStorage>();

        storage.maxSlots = 3;
        storage.items = new WagonStorage.StoredItem[storage.maxSlots];
    }

    [TearDown]
    public void TearDown()
    {
        if (storageObject != null)
        {
            Object.DestroyImmediate(storageObject);
        }
    }

    [Test]
    public void IsSlotEmpty_InvalidIndex_ReturnsFalse()
    {
        Assert.IsFalse(storage.IsSlotEmpty(-1));
        Assert.IsFalse(storage.IsSlotEmpty(99));
    }

    [Test]
    public void IsValidIndex_ReturnsTrue_OnlyWithinBounds()
    {
        Assert.IsFalse(storage.IsValidIndex(-1));
        Assert.IsTrue(storage.IsValidIndex(0));
        Assert.IsTrue(storage.IsValidIndex(1));
        Assert.IsTrue(storage.IsValidIndex(2));
        Assert.IsFalse(storage.IsValidIndex(3));
    }

    [Test]
    public void NewStorage_AllSlotsAreEmpty()
    {
        Assert.IsTrue(storage.IsSlotEmpty(0));
        Assert.IsTrue(storage.IsSlotEmpty(1));
        Assert.IsTrue(storage.IsSlotEmpty(2));
    }

    [Test]
    public void RemoveItemAt_InvalidIndex_ReturnsFalse()
    {
        bool removed = storage.RemoveItemAt(10);

        Assert.IsFalse(removed);
    }

    [Test]
    public void RemoveItemAt_ValidIndex_ReturnsTrue_AndEmptiesSlot()
    {
        GameObject item = new GameObject("Item");
        storage.TryAddItemToSlot(2, item);

        bool removed = storage.RemoveItemAt(2);

        Assert.IsTrue(removed);
        Assert.IsTrue(storage.IsSlotEmpty(2));

        Object.DestroyImmediate(item);
    }

    [Test]
    public void TryAddItemToSlot_InvalidIndex_ReturnsFalse()
    {
        GameObject item = new GameObject("Item");

        bool result = storage.TryAddItemToSlot(99, item);

        Assert.IsFalse(result);

        Object.DestroyImmediate(item);
    }

    [Test]
    public void TryAddItemToSlot_NullItem_ReturnsFalse()
    {
        bool result = storage.TryAddItemToSlot(0, null);

        Assert.IsFalse(result);
        Assert.IsTrue(storage.IsSlotEmpty(0));
    }

    [Test]
    public void TryAddItemToSlot_OccupiedSlot_ReturnsFalse()
    {
        GameObject itemA = new GameObject("ItemA");
        GameObject itemB = new GameObject("ItemB");

        bool first = storage.TryAddItemToSlot(0, itemA);
        bool second = storage.TryAddItemToSlot(0, itemB);

        Assert.IsTrue(first);
        Assert.IsFalse(second);

        bool found = storage.TryGetItemAt(0, out GameObject storedItem);
        Assert.IsTrue(found);
        Assert.AreSame(itemA, storedItem);

        Object.DestroyImmediate(itemA);
        Object.DestroyImmediate(itemB);
    }

    [Test]
    public void TryAddItemToSlot_ValidEmptySlot_ReturnsTrue_AndStoresItem()
    {
        GameObject item = new GameObject("Item");

        bool result = storage.TryAddItemToSlot(1, item);

        Assert.IsTrue(result);
        Assert.IsFalse(storage.IsSlotEmpty(1));

        bool found = storage.TryGetItemAt(1, out GameObject storedItem);

        Assert.IsTrue(found);
        Assert.AreSame(item, storedItem);

        Object.DestroyImmediate(item);
    }

    [Test]
    public void TryGetItemAt_EmptySlot_ReturnsFalse_AndNull()
    {
        bool result = storage.TryGetItemAt(1, out GameObject item);

        Assert.IsFalse(result);
        Assert.IsNull(item);
    }
}
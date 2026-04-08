using NUnit.Framework;
using UnityEngine;

public class PlayerInventoryAmmoTests
{
    private GameObject inventoryObject;
    private PlayerInventory inventory;

    [SetUp]
    public void SetUp()
    {
        inventoryObject = new GameObject("PlayerInventory");
        inventory = inventoryObject.AddComponent<PlayerInventory>();

        inventory.storedHandgunAmmo = 0;
        inventory.storedShotgunAmmo = 0;
        inventory.storedRifleAmmo = 0;
    }

    [TearDown]
    public void TearDown()
    {
        if (inventoryObject != null)
        {
            Object.DestroyImmediate(inventoryObject);
        }
    }

    [Test]
    public void AddAmmo_Handgun_IncreasesOnlyHandgunReserve()
    {
        inventory.AddAmmo(AmmoType.Handgun, 20);

        Assert.AreEqual(20, inventory.GetReserveAmmo(AmmoType.Handgun));
        Assert.AreEqual(0, inventory.GetReserveAmmo(AmmoType.Shotgun));
        Assert.AreEqual(0, inventory.GetReserveAmmo(AmmoType.Rifle));
    }

    [Test]
    public void AddAmmo_Shotgun_IncreasesOnlyShotgunReserve()
    {
        inventory.AddAmmo(AmmoType.Shotgun, 8);

        Assert.AreEqual(0, inventory.GetReserveAmmo(AmmoType.Handgun));
        Assert.AreEqual(8, inventory.GetReserveAmmo(AmmoType.Shotgun));
        Assert.AreEqual(0, inventory.GetReserveAmmo(AmmoType.Rifle));
    }

    [Test]
    public void AddAmmo_Rifle_IncreasesOnlyRifleReserve()
    {
        inventory.AddAmmo(AmmoType.Rifle, 30);

        Assert.AreEqual(0, inventory.GetReserveAmmo(AmmoType.Handgun));
        Assert.AreEqual(0, inventory.GetReserveAmmo(AmmoType.Shotgun));
        Assert.AreEqual(30, inventory.GetReserveAmmo(AmmoType.Rifle));
    }

    [Test]
    public void AddAmmo_ZeroAmount_DoesNothing()
    {
        inventory.AddAmmo(AmmoType.Rifle, 0);

        Assert.AreEqual(0, inventory.GetReserveAmmo(AmmoType.Rifle));
    }

    [Test]
    public void AddAmmo_NegativeAmount_DoesNothing()
    {
        inventory.AddAmmo(AmmoType.Rifle, -10);

        Assert.AreEqual(0, inventory.GetReserveAmmo(AmmoType.Rifle));
    }

    [Test]
    public void ConsumeAmmo_WhenEnoughAmmo_ReturnsRequestedAmount_AndDecreasesReserve()
    {
        inventory.AddAmmo(AmmoType.Shotgun, 12);

        int taken = inventory.ConsumeAmmo(AmmoType.Shotgun, 5);

        Assert.AreEqual(5, taken);
        Assert.AreEqual(7, inventory.GetReserveAmmo(AmmoType.Shotgun));
    }

    [Test]
    public void ConsumeAmmo_WhenNotEnoughAmmo_ReturnsOnlyAvailableAmount()
    {
        inventory.AddAmmo(AmmoType.Handgun, 4);

        int taken = inventory.ConsumeAmmo(AmmoType.Handgun, 10);

        Assert.AreEqual(4, taken);
        Assert.AreEqual(0, inventory.GetReserveAmmo(AmmoType.Handgun));
    }

    [Test]
    public void ConsumeAmmo_ZeroAmount_ReturnsZero_AndDoesNotChangeReserve()
    {
        inventory.AddAmmo(AmmoType.Rifle, 9);

        int taken = inventory.ConsumeAmmo(AmmoType.Rifle, 0);

        Assert.AreEqual(0, taken);
        Assert.AreEqual(9, inventory.GetReserveAmmo(AmmoType.Rifle));
    }

    [Test]
    public void ConsumeAmmo_NegativeAmount_ReturnsZero_AndDoesNotChangeReserve()
    {
        inventory.AddAmmo(AmmoType.Rifle, 9);

        int taken = inventory.ConsumeAmmo(AmmoType.Rifle, -3);

        Assert.AreEqual(0, taken);
        Assert.AreEqual(9, inventory.GetReserveAmmo(AmmoType.Rifle));
    }
}
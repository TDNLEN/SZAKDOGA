using UnityEngine;

public class ItemShopPanel : MonoBehaviour
{
    private FuelShop currentFuelShop;
    private HealShop currentHealShop;
    private WeaponShop currentWeaponShop;

    public void SetFuelShop(FuelShop shop)
    {
        currentFuelShop = shop;
        currentHealShop = null;
        currentWeaponShop = null;
    }

    public void SetHealShop(HealShop shop)
    {
        currentFuelShop = null;
        currentHealShop = shop;
        currentWeaponShop = null;
    }

    public void SetWeaponShop(WeaponShop shop)
    {
        currentFuelShop = null;
        currentHealShop = null;
        currentWeaponShop = shop;
    }

    public void BuyItem(int index)
    {
        if (currentFuelShop != null)
        {
            currentFuelShop.BuyItem(index);
            return;
        }

        if (currentHealShop != null)
        {
            currentHealShop.BuyItem(index);
            return;
        }

        if (currentWeaponShop != null)
        {
            currentWeaponShop.BuyItem(index);
            return;
        }

        Debug.LogWarning("[ItemShopPanel] Nincs aktuális shop beállítva.");
    }

    public void CloseCurrentShop()
    {
        if (currentFuelShop != null)
        {
            currentFuelShop.CloseShop();
            return;
        }

        if (currentHealShop != null)
        {
            currentHealShop.CloseShop();
            return;
        }

        if (currentWeaponShop != null)
        {
            currentWeaponShop.CloseShop();
            return;
        }

        Debug.LogWarning("[ItemShopPanel] Nincs aktuális shop beállítva.");
    }
}
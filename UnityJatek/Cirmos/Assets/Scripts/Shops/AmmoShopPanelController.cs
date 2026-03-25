using UnityEngine;

public class AmmoShopPanelController : MonoBehaviour
{
    private AmmoShop currentAmmoShop;

    public void SetAmmoShop(AmmoShop shop)
    {
        currentAmmoShop = shop;
    }

    public void BuyAmmo(int index)
    {
        if (currentAmmoShop != null)
        {
            currentAmmoShop.BuyAmmo(index);
            return;
        }

        Debug.LogWarning("[AmmoShopPanelController] Nincs aktuális AmmoShop beállítva.");
    }

    public void CloseCurrentShop()
    {
        if (currentAmmoShop != null)
        {
            currentAmmoShop.CloseShop();
            return;
        }

        Debug.LogWarning("[AmmoShopPanelController] Nincs aktuális AmmoShop beállítva.");
    }
}
using UnityEngine;
using TMPro;

public class PlayerWallet : MonoBehaviour
{
    [Header("Money")]
    public int coins = 0;                     // kezdetben 0 pénz

    [Header("UI")]
    public TextMeshProUGUI coinText;         // ide húzd a UI szöveget (pl. "CoinsText")
    public GameObject coinPanel;             // opcionális: a háttér panel (ha van)

    private void Start()
    {
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        coins += amount;
        UpdateUI();
    }

    public bool TrySpend(int amount)
    {
        if (coins < amount) return false;

        coins -= amount;
        UpdateUI();
        return true;
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = coins.ToString();

        // ha akarod: panel elrejtése, ha 0 a pénz
        if (coinPanel != null)
            coinPanel.SetActive(true);   // vagy: coins > 0-ra állítsd, ha úgy akarod eltüntetni
    }
}

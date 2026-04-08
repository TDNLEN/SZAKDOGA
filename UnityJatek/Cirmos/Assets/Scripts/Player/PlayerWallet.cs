using UnityEngine;
using TMPro;

public class PlayerWallet : MonoBehaviour
{
    [Header("Money")]
    public int coins = 0;                 

    [Header("UI")]
    public TextMeshProUGUI coinText;        
    public GameObject coinPanel;          

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

        if (coinPanel != null)
            coinPanel.SetActive(true);  
    }
}

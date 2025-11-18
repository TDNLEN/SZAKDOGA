using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CoinPickup : MonoBehaviour
{
    public int value = 10;   // ennyit ér ez az egy coin

    private void Reset()
    {
        var c = GetComponent<Collider2D>();
        if (c) c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var wallet = other.GetComponent<PlayerWallet>();
        if (wallet == null)
            wallet = other.GetComponentInParent<PlayerWallet>();

        if (wallet == null)
        {
            Debug.LogWarning("Nincs PlayerWallet a playeren, nem tudom hozzáadni a coint.");
            return;
        }

        wallet.AddCoins(value);

        // coin eltûnik
        Destroy(gameObject);
    }
}

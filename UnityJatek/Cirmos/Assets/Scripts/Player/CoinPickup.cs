using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CoinPickup : MonoBehaviour
{
    public int value = 10;

    [Header("Audio")]
    public AudioClip coinSound;
    [Range(0f, 1f)] public float coinVolume = 1f;

    private void Reset()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerWallet wallet = other.GetComponent<PlayerWallet>();
        if (wallet == null)
            wallet = other.GetComponentInParent<PlayerWallet>();

        if (wallet == null)
        {
            Debug.LogWarning("Nincs PlayerWallet a playeren, nem tudom hozzáadni a coint.");
            return;
        }

        wallet.AddCoins(value);

        PlayCoinSound();

        Destroy(gameObject);
    }

    private void PlayCoinSound()
    {
        if (coinSound == null) return;

        AudioSource.PlayClipAtPoint(coinSound, transform.position, coinVolume);
    }
}
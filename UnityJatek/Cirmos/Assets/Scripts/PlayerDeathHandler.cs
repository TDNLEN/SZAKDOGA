using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Refs")]
    public GameOverUI gameOverUI;
    public Rigidbody2D rb;

    [Header("Disable On Death")]
    public MonoBehaviour[] scriptsToDisable;

    private bool isDead = false;

    public bool IsDead => isDead;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (gameOverUI == null)
            gameOverUI = FindFirstObjectByType<GameOverUI>();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        for (int i = 0; i < scriptsToDisable.Length; i++)
        {
            if (scriptsToDisable[i] != null)
                scriptsToDisable[i].enabled = false;
        }

        if (gameOverUI != null)
            gameOverUI.ShowGameOver();

        Time.timeScale = 0f;
    }
}
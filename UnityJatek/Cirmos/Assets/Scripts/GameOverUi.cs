using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("UI")]
    public Image deathOverlay;
    public GameObject deathButtonPanel;

    [Header("Overlay Animation")]
    [Range(0f, 1f)] public float finalOverlayAlpha = 0.45f;
    [Range(0f, 1f)] public float flashAlpha = 0.85f;
    public float flashDuration = 0.12f;
    public float settleDuration = 0.25f;

    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";

    private bool isShowing = false;

    private void Start()
    {
        if (deathOverlay != null)
        {
            Color c = deathOverlay.color;
            c.a = 0f;
            deathOverlay.color = c;

            deathOverlay.raycastTarget = false;
            deathOverlay.gameObject.SetActive(false);
        }

        if (deathButtonPanel != null)
            deathButtonPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (isShowing) return;
        isShowing = true;
        StartCoroutine(ShowGameOverRoutine());
    }

    private IEnumerator ShowGameOverRoutine()
    {
        if (deathOverlay != null)
        {
            deathOverlay.gameObject.SetActive(true);
            deathOverlay.raycastTarget = false;

            Color c = deathOverlay.color;
            c.a = 0f;
            deathOverlay.color = c;

            float t = 0f;
            while (t < flashDuration)
            {
                t += Time.unscaledDeltaTime;
                float lerp = Mathf.Clamp01(t / flashDuration);
                c.a = Mathf.Lerp(0f, flashAlpha, lerp);
                deathOverlay.color = c;
                yield return null;
            }

            t = 0f;
            while (t < settleDuration)
            {
                t += Time.unscaledDeltaTime;
                float lerp = Mathf.Clamp01(t / settleDuration);
                c.a = Mathf.Lerp(flashAlpha, finalOverlayAlpha, lerp);
                deathOverlay.color = c;
                yield return null;
            }

            c.a = finalOverlayAlpha;
            deathOverlay.color = c;
        }

        if (deathButtonPanel != null)
            deathButtonPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        Debug.Log("Trying to load menu scene: " + mainMenuSceneName);
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Refs")]
    public GameObject pauseMenuUI; // ide húzd be a PauseMenu GameObjectet
    public CanvasGroup dimBackground; // opcionális, ha fokozatosan sötétíted

    private bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseMenuUI == null) return;

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // játék megáll
        isPaused = true;

        // ha van CanvasGroup az elsötétítéshez:
        if (dimBackground != null)
        {
            dimBackground.alpha = 0.5f; // áttetszõ fekete
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuUI == null) return;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // játék újraindul
        isPaused = false;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // biztos, ami biztos
        SceneManager.LoadScene("Main_menu"); // vissza a fõmenübe
    }
}

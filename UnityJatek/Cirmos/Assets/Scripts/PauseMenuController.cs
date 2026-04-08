using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Refs")]
    public GameObject pauseMenuUI; 
    public CanvasGroup dimBackground; 

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
        Time.timeScale = 0f; 
        isPaused = true;

        if (dimBackground != null)
        {
            dimBackground.alpha = 0.5f; 
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuUI == null) return;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Main_menu"); 
    }
}

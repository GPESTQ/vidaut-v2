using UnityEngine;

public class ExitInicial : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    public static bool isPaused = false;

    private void Start()
    {
        if (menu != null)
            menu.SetActive(false);

        isPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                FecharMenuInicial();
            else
                AbrirMenu();
        }
    }

    public void AbrirMenu()
    {
        if (menu == null)
            return;

        menu.SetActive(true);
        PauseGame();
    }

    public void FecharMenuInicial()
    {
        if (menu == null)
            return;

        menu.SetActive(false);
        ResumeGame();
    }

    public void SairJogo()
    {
        Application.Quit();
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}

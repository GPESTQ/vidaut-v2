using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public GameObject menu;
    public MinigameManeger mg;
    [HideInInspector] public static bool isPaused = false;

    public GameObject Indicador;

    private MiniGameBase minigameAntesDoPause;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                FecharMenu();
            else
                AbrirMenu();
        }
    }

    public void AbrirMenu()
    {
        minigameAntesDoPause = mg.GetMinigameAtivo();
        mg.gameObject.SetActive(false);
        menu.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void FecharMenu()
    {
        if (menu == null)
            return;

        menu.SetActive(false);
        mg.gameObject.SetActive(true);

        if (minigameAntesDoPause != null)
            mg.ReativarSemReiniciar(minigameAntesDoPause);

        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void SairDoJogo()
    {
        Application.Quit();
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}

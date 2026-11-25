using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManeger : MonoBehaviour
{
    MiniGameBase[] MiniGames;

    void Awake()
    {
        MiniGames = FindObjectsOfType<MiniGameBase>(true);
        DesativarTodos();
    }

    public void Ativar(MiniGameBase alvo)
    {
        if (MiniGames == null)
            MiniGames = FindObjectsOfType<MiniGameBase>(true);

        foreach (var mg in MiniGames)
        {
            mg.gameObject.SetActive(false);
        }

        alvo.gameObject.SetActive(true);
        alvo.Iniciar();
    }

    public void DesativarTodos()
    {
        if (MiniGames == null)
            MiniGames = FindObjectsOfType<MiniGameBase>(true);

        foreach (var mg in MiniGames)
        {
            mg.Resetar();
            mg.gameObject.SetActive(false);
        }
    }

    public MiniGameBase GetMinigameAtivo()
    {
        if (MiniGames == null)
            MiniGames = FindObjectsOfType<MiniGameBase>(true);

        foreach (var mg in MiniGames)
        {
            if (mg.gameObject.activeSelf)
                return mg;
        }
        return null;
    }

    public void ReativarSemReiniciar(MiniGameBase alvo)
    {
        if (MiniGames == null)
            MiniGames = FindObjectsOfType<MiniGameBase>(true);

        foreach (var mg in MiniGames)
            mg.gameObject.SetActive(false);

        alvo.gameObject.SetActive(true);
    }
}

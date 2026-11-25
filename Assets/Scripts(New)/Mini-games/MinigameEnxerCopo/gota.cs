using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gota : MonoBehaviour
{
    private MinigameEnxer minigame;

    public void Init(MinigameEnxer mg)
    {
        minigame = mg;
    }
    private void Update()
    {
        if (minigame == null || !minigame.gameObject.activeInHierarchy)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Copo"))
        {
            if (minigame == null)
            {
                Destroy(gameObject);
                return;
            }

            minigame.RegistrarGota();
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("LimiteMapa"))
        {
            Destroy(gameObject); // gota saiu da tela sem acertar o copo
        }
    }
}
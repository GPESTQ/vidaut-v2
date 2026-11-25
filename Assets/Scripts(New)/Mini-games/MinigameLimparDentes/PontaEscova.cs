using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PontaEscova : MonoBehaviour
{
    private PlacaDentaria placaAtual;

    void OnTriggerEnter2D(Collider2D other)
    {
        var placa = other.GetComponent<PlacaDentaria>();
        if (placa != null)
        {
            placaAtual = placa;
            placa.EstaSendoLimpa = true; // começa
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var placa = other.GetComponent<PlacaDentaria>();

        if (placa != null && placa == placaAtual)
        {
            placa.Limpar(1f);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var placa = other.GetComponent<PlacaDentaria>();

        if (placa != null && placa == placaAtual)
        {
            placa.PararLimpeza(); // 🔥 ESSENCIAL
            placaAtual = null;
        }
    }

    void Awake()
    {
        gameObject.tag = "Escova";
    }
}
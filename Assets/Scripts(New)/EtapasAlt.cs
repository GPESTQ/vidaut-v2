using System.Collections;
using System.Collections.Generic;

using UnityEngine;
[System.Serializable]
public class EtapasAlt
{

    public string nome;

    [Header("Introducao da Etapa")]
    public Fala[] Falasintroducao;

    [Header("Finalizacao da Etapa")]
    public Fala[] falasFinalizacao;

    [Header("Passos da Etapa")]
    public PassoAlt[] passos;

    [HideInInspector]  public bool concluida;
    [HideInInspector] public bool Desbloqueada;
    [HideInInspector] public int ultimoPassoConcluido = -1;

}

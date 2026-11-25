using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PassoAlt
{
    public TipoConclusao tipoConclusao;

    [Header("Mini-game")]
    public MiniGameBase minigame;

    [Header("Alvos simples")]
    public GameObject objetoAlvo;
    public Transform destino;

    [Header("Falas do passo")]
    public Fala[] falas;
    public Fala[] falasFinalizacao;
    [HideInInspector] public bool concluido;

    [Header("Snapshot")]
    [Tooltip("Se desmarcado, este passo não grava (nem usa) snapshot dos objetos da cena.")]
    public bool salvarSnapshot = true;

    [Header("Objetos controlados pelo passo")]
    public UnityEvent aoIniciar;
    public UnityEvent aoConcluir;
}
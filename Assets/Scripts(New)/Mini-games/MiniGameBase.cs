using UnityEngine;
using UnityEngine.Events;

public class MiniGameBase : MonoBehaviour
{
    public UnityEvent onConcluido;
    protected bool iniciado;
    protected bool concluido;

    public virtual void Iniciar()
    {
        concluido = false;
        iniciado = true;
    }

    public virtual void Resetar()
    {
        iniciado = false;
        concluido = false;
        StopAllCoroutines();
        onConcluido?.RemoveAllListeners();
    }

    protected void Concluir()
    {
        if (concluido) return;
        concluido = true;
        iniciado = false;
        onConcluido?.Invoke();
    }
}
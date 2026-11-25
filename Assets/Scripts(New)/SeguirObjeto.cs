using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SeguirObjeto : MonoBehaviour
{
    [Header("Modo Alvos")]
    public Transform alvoInicio;
    public Transform alvoFim;

    [Header("Modo Arrastar")]
    public bool usarArrastar = false;
    public bool loopArraste = false;
    public float tempoArrastar = 1f;
    public float pausaEmB = 0.25f;
    public float tempoRetornoParaA = 0.03f;

    [Header("Flutuação")]
    public float amplitude = 0.5f;
    public float velocidadeSobeDesce = 1f;
    public Vector3 offset = new Vector3(0, 1f, 0);
    public float suavizacaoPosicao = 10f;

    [Header("Trajeto Visual")]
    public LineRenderer linha;

    Coroutine arrasteCoroutine;
    bool usarTrajeto;

    void Update()
    {
        if (usarArrastar || usarTrajeto || alvoInicio == null)
            return;

        // 🔹 seguir alvo normalmente (clique / foco)
        Vector3 alvoPos = alvoInicio.position + offset;
        float yOffset = Mathf.Sin(Time.time * velocidadeSobeDesce) * amplitude;
        Vector3 destino = alvoPos + new Vector3(0, yOffset, 0);

        transform.position = Vector3.Lerp(
            transform.position,
            destino,
            Time.deltaTime * suavizacaoPosicao
        );
    }

    void LateUpdate()
    {
        if (!usarTrajeto || alvoInicio == null || alvoFim == null || linha == null)
            return;

        linha.SetPosition(0, alvoInicio.position);
        linha.SetPosition(1, alvoFim.position);
    }

    IEnumerator LoopAparaB()
    {
        yield return AnimarDePara(alvoInicio, alvoFim, tempoArrastar);

        float t = 0f;
        while (t < pausaEmB)
        {
            t += Time.deltaTime;
            yield return null;
        }

        if (loopArraste)
        {
            yield return AnimarDePara(alvoFim, alvoInicio, tempoRetornoParaA);
            arrasteCoroutine = StartCoroutine(LoopAparaB());
        }
        else
        {
            usarArrastar = false;
            arrasteCoroutine = null;
        }
    }

    IEnumerator AnimarDePara(Transform fromT, Transform toT, float duracao)
    {
        if (fromT == null || toT == null) yield break;

        Vector3 start = fromT.position + offset;
        Vector3 end = toT.position + offset;
        float t = 0f;

        while (t < duracao)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duracao);
            transform.position = Vector3.Lerp(start, end, p);
            yield return null;
        }

        transform.position = end;
    }

    // ======================
    // API PÚBLICA (GameManager)
    // ======================

    // 🔹 Modo normal
    public void SeguirTransform(Transform alvo)
    {
        LimparEstado();

        alvoInicio = alvo;
        usarTrajeto = false;

        if (linha != null)
            linha.enabled = false;

        gameObject.SetActive(true);
    }

    // 🔹 Modo arrastável A → B
    public void MostrarTrajeto(Transform inicio, Transform fim, bool loop = true)
    {
        LimparEstado();

        alvoInicio = inicio;
        alvoFim = fim;
        usarTrajeto = true;
        usarArrastar = true;
        loopArraste = loop;

        if (linha != null)
            linha.enabled = true;

        transform.position = inicio.position + offset;
        arrasteCoroutine = StartCoroutine(LoopAparaB());

        gameObject.SetActive(true);
    }

    public void PararEEsconder()
    {
        LimparEstado();
        gameObject.SetActive(false);
    }

    void LimparEstado()
    {
        if (arrasteCoroutine != null)
            StopCoroutine(arrasteCoroutine);

        arrasteCoroutine = null;
        usarArrastar = false;
        usarTrajeto = false;
        loopArraste = false;
        alvoInicio = null;
        alvoFim = null;

        if (linha != null)
            linha.enabled = false;
    }
}

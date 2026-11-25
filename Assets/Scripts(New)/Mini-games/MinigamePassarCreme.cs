using System.Collections;
using UnityEngine;

public class MinigamePassarCreme : MiniGameBase
{
    [Header("Referências")]
    public SpriteRenderer escovaRenderer;
    public Collider2D areaCreme;
    public LineRenderer line;

    [Header("Sprites")]
    public Sprite escovaSemCreme;
    public Sprite escovaComCreme;

    [Header("Preenchimento")]
    public float distanciaNecessaria = 8f;
    public float distanciaMinima = 0.1f;
    bool encerrando;
    float distanciaAtual;
    Vector2 ultimoPonto;
    bool temUltimoPonto;
    public float delayConclusao = 4f;

    void Update()
    {
        if (concluido) return;
        if(encerrando) return;

        if (!InputHelper.GetButton())
        {
            temUltimoPonto = false;
            return;
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(InputHelper.GetPosition());

        if (!areaCreme.OverlapPoint(mousePos))
        {
            temUltimoPonto = false;
            return;
        }

        if (temUltimoPonto)
        {
            float d = Vector2.Distance(ultimoPonto, mousePos);

            if (d >= distanciaMinima)
            {
                distanciaAtual += d;
                AdicionarPonto(mousePos);

                if (distanciaAtual >= distanciaNecessaria)
                {
                    ConcluirMinigame();
                }
            }
        }

        ultimoPonto = mousePos;
        temUltimoPonto = true;
    }

    public override void Iniciar()
    {
        base.Iniciar();

        StopAllCoroutines();

        distanciaAtual = 0f;
        temUltimoPonto = false;
        ultimoPonto = Vector2.zero;
        concluido = false;
        encerrando = false;
        escovaRenderer.sprite = escovaSemCreme;
        line.positionCount = 0;
    }

    void AdicionarPonto(Vector2 pos)
    {
        line.positionCount++;
        line.SetPosition(line.positionCount - 1, pos);
    }

    void ConcluirMinigame()
    {
        if (encerrando) return;
        encerrando = true;

        line.positionCount = 0;
        escovaRenderer.sprite = escovaComCreme;

        StartCoroutine(DelayConclusao());   
    }

    IEnumerator DelayConclusao()
    {
        yield return new WaitForSeconds(delayConclusao);
        Concluir();
    }
}

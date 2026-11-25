// PlacaDentaria.cs
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class PlacaDentaria : MonoBehaviour
{
    public static Action<PlacaDentaria> onPlacaLimpa;

    public bool EstaSendoLimpa { get; set; }
    public bool EstaLimpa => jaFoiLimpa;

    [Range(0f, 1f)]
    public float placaAtual = 1f;
    public float velocidadeLimpeza = 0.40f;
    public bool resetavel = true;

    private SpriteRenderer sr;
    private Collider2D col;
    private bool jaFoiLimpa = false;
    private bool finalizando = false;

    void Awake()
    {
        sr  = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        AtualizarVisual();
    }

    public void Limpar(float intensidade)
    {
        if (jaFoiLimpa) return;

        EstaSendoLimpa = true;
        placaAtual -= intensidade * velocidadeLimpeza * Time.deltaTime;
        placaAtual  = Mathf.Clamp01(placaAtual);
        AtualizarVisual();

        if (placaAtual <= 0.80f)
        {
            jaFoiLimpa     = true;
            EstaSendoLimpa = false;
            col.enabled    = false;

            onPlacaLimpa?.Invoke(this);

            if (!finalizando)
            {
                finalizando = true;
                StartCoroutine(FinalizarPlaca());
            }
        }
    }

    public void Resetar()
    {
        if (!resetavel) return;

        // para coroutine de finalização se estiver rodando
        StopAllCoroutines();

        jaFoiLimpa  = false;
        finalizando = false;
        placaAtual  = 1f;

        // reativa tudo
        gameObject.SetActive(true);
        sr.enabled  = true;
        col.enabled = true;

        AtualizarVisual();
    }

    public void PararLimpeza()
    {
        EstaSendoLimpa = false;
    }

    void AtualizarVisual()
    {
        if (sr == null) return;
        Color c = sr.color;
        c.a = placaAtual;
        sr.color = c;
    }

    IEnumerator FinalizarPlaca()
    {
        float t        = 0f;
        float duracao  = 0.3f;
        Color original = sr.color;

        while (t < duracao)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(original.a, 0f, t / duracao);
            sr.color = new Color(original.r, original.g, original.b, alpha);
            yield return null;
        }

        sr.enabled = false;
    }
}
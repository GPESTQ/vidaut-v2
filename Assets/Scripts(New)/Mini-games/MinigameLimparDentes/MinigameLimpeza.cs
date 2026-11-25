using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinigameLimpeza : MiniGameBase
{
    [Header("Spawn de sujeiras")]
    public BoxCollider2D areaspawncima;
    public BoxCollider2D areaspawnbaixo;
    public GameObject[] prefabSujeira;
    public int quantidadePorArea;

    [Header("Container sujeiras")]
    public Transform containerSujeiras;

    [Header("Placas")]
    public List<PlacaDentaria> placas = new List<PlacaDentaria>();

    public float tempo = 0.0f;

    private int sujeirasRestantes;
    private bool verificandoFim = false;

    void Awake()
    {
        if (containerSujeiras == null)
        {
            GameObject obj = new GameObject("ContainerSujeiras");
            obj.transform.SetParent(transform);
            containerSujeiras = obj.transform;
        }
    }

    public override void Iniciar()
    {
        base.Iniciar();

        verificandoFim = false;

        PlacaDentaria.onPlacaLimpa -= AoLimparPlaca;
        PlacaDentaria.onPlacaLimpa += AoLimparPlaca;
        Sujeira.onSujeiraRemovida  -= SujeiraRemovida;
        Sujeira.onSujeiraRemovida  += SujeiraRemovida;

        placas.Clear();
        placas.AddRange(GetComponentsInChildren<PlacaDentaria>(true));

        foreach (var p in placas)
            p.Resetar();

        for (int i = containerSujeiras.childCount - 1; i >= 0; i--)
            Destroy(containerSujeiras.GetChild(i).gameObject);

        sujeirasRestantes = 0;
        SpawnSujeiras(areaspawncima);
        SpawnSujeiras(areaspawnbaixo);
    }

    public override void Resetar()
    {
        base.Resetar();

        verificandoFim = false;

        Sujeira.onSujeiraRemovida  -= SujeiraRemovida;
        PlacaDentaria.onPlacaLimpa -= AoLimparPlaca;

        if (containerSujeiras != null)
            for (int i = containerSujeiras.childCount - 1; i >= 0; i--)
                Destroy(containerSujeiras.GetChild(i).gameObject);

        sujeirasRestantes = 0;
    }

    void OnDestroy()
    {
        PlacaDentaria.onPlacaLimpa -= AoLimparPlaca;
        Sujeira.onSujeiraRemovida -= SujeiraRemovida;
    }

    void SpawnSujeiras(BoxCollider2D area)
    {
        if (area == null || containerSujeiras == null) return;

        for (int i = 0; i < quantidadePorArea; i++)
        {
            Vector2 pos = PontoAleatorioNaArea(area);
            Instantiate(
                prefabSujeira[Random.Range(0, prefabSujeira.Length)],
                pos,
                Quaternion.identity,
                containerSujeiras
            );
            sujeirasRestantes++;
        }
    }

    void SujeiraRemovida()
    {
        sujeirasRestantes--;
        VerificarFim();
    }

    void AoLimparPlaca(PlacaDentaria placa)
    {
        VerificarFim();
    }

    public void VerificarFim()
    {
        if (concluido || verificandoFim) return;
        if (!iniciado) return;

        int placasRestantes = 0;
        foreach (var p in placas)
            if (p != null && !p.EstaLimpa && p.gameObject.activeInHierarchy)
                placasRestantes++;

        if (placasRestantes == 0 && sujeirasRestantes <= 0)
        {
            verificandoFim = true;
            StartCoroutine(TempoFinal());
        }
    }

    IEnumerator TempoFinal()
    {
        yield return new WaitForSeconds(tempo);
        Concluir();
    }

    Vector2 PontoAleatorioNaArea(BoxCollider2D area)
    {
        Bounds b = area.bounds;
        return new Vector2(
            Random.Range(b.min.x, b.max.x),
            Random.Range(b.min.y, b.max.y)
        );
    }
}

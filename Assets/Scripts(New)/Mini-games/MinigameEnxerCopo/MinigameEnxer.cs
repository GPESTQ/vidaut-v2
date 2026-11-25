using System.Collections;
using UnityEngine;

public class MinigameEnxer : MiniGameBase
{
    [Header("Configuração")]
    public GameObject prefabGota;
    public Transform pontoSpawn;
    public float intervaloSpawn = 0.5f;
    public int gotasNecessarias = 10;

    [Header("Estado")]
    public int gotasNoCopo = 0;
    private bool gerando;

    [Header("Visual do Copo")]
    public SpriteRenderer copoSprite;
    public Sprite[] estadosCopo;

    private CopoController copoController;
    private Vector3 copoPosicaoInicial;

    void Awake()
    {
        copoController = GetComponentInChildren<CopoController>(true);
        if (copoController != null)
            copoPosicaoInicial = copoController.transform.position;
    }

    public override void Iniciar()
    {
        StopAllCoroutines();

        base.Iniciar();

        if (copoController != null)
        {
            copoController.transform.position = copoPosicaoInicial;
            var rb = copoController.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.position = copoPosicaoInicial;
                rb.velocity = Vector2.zero;
            }
        }

        gotasNoCopo = 0;
        gerando = true;
        AtualizarVisual();

        StartCoroutine(GerarGotas());
    }

    public override void Resetar()
    {
        base.Resetar();
    }

    IEnumerator GerarGotas()
    {
        while (gerando)
        {
            SpawnGota();
            yield return new WaitForSeconds(intervaloSpawn);
        }
    }

    private void OnDisable()
    {
        gerando = false;
    }

    void SpawnGota()
    {
        Vector3 pos = pontoSpawn.position;
        pos.z = 0f;

        GameObject g = Instantiate(prefabGota, pos, Quaternion.identity);
        g.transform.localScale = Vector3.one * 3.5f;

        var sr = g.GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Minigame";
        sr.sortingOrder = 99;

        var script = g.GetComponent<gota>();
        if (script == null)
            return;

        script.Init(this);
    }

    public void RegistrarGota()
    {
        if (!gerando) return;

        gotasNoCopo++;

        AtualizarVisual();

        if (gotasNoCopo >= gotasNecessarias)
        {
            gerando = false;
            StopAllCoroutines();
            Concluir();
        }
    }

    void AtualizarVisual()
    {
        if (estadosCopo.Length == 0) return;

        float progresso = (float)gotasNoCopo / gotasNecessarias;
        int index = Mathf.Clamp(
            Mathf.FloorToInt(progresso * (estadosCopo.Length - 1)),
            0,
            estadosCopo.Length - 1
        );

        copoSprite.sprite = estadosCopo[index];
    }
}

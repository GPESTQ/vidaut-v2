using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamemaneger : MonoBehaviour
{
    public SeguirObjeto Indicador;
    public static Gamemaneger instance;
    public MinigameManeger minigameManager;
    public ProgressoSnapShot ultimoSnapShot;
    public EtapasAlt[] etapas;
    public Text textUI;
    public AudioSource audioSource;

    [HideInInspector] public int etapaAtual;
    [HideInInspector] public int PassoAtualPublico => passoAtual;
    int passoAtual;
    bool aguardandoFala;
    bool bloquearInput;
    bool minigameAtivo = false;

    private Coroutine falaAtualCoroutine;
    private SaveData saveAtual;

    static string NomeSafe(UnityEngine.Object obj) =>
        (obj != null && obj) ? obj.name : "(nulo)";

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        if (ultimoSnapShot == null)
            ultimoSnapShot = new ProgressoSnapShot();

        saveAtual = SaveSystem.Carregar() ?? new SaveData();

        if (saveAtual.etapas.Count > 0)
            CarregarSave();
        else
        {
            if (etapas != null && etapas.Length > 0)
            {
                etapas[0].Desbloqueada = true;
                StartCoroutine(InicarComDelay());
            }
        }
    }

    public void SalvarEmDisco()
    {
        saveAtual.etapaAtual = etapaAtual;
        saveAtual.passoAtual = passoAtual;

        saveAtual.etapas.Clear();
        foreach (var e in etapas)
        {
            var es = new EtapaSalva
            {
                desbloqueada         = e.Desbloqueada,
                concluida            = e.concluida,
                ultimoPassoConcluido = e.ultimoPassoConcluido
            };
            foreach (var p in e.passos)
                es.passosConcluidos.Add(p.concluido);
            saveAtual.etapas.Add(es);
        }

        SaveSystem.Salvar(saveAtual);
    }

    void GravarSnapshotDoPasso(int etapa, int passo)
    {
        var resetaveis = FindObjectsOfType<Resetavel>(true);
        var lista = new List<ObjetoSalvo>();
        foreach (var r in resetaveis)
        {
            var dado = r.ExportarParaSave();
            if (dado != null)
                lista.Add(dado);
        }

        SaveSystem.GravarSnapshot(saveAtual, etapa, passo, lista);
    }

    void RestaurarSnapshotDoPasso(int etapa, int passo)
    {
        var snap = SaveSystem.EncontrarSnapshot(saveAtual, etapa, passo);
        var resetaveis = FindObjectsOfType<Resetavel>(true);

        if (snap == null || snap.objetos.Count == 0)
            return;

        foreach (var dado in snap.objetos)
        {
            foreach (var r in resetaveis)
            {
                if (r.ObterID() == dado.id)
                {
                    r.AplicarDoSave(dado);
                    break;
                }
            }
        }
    }

    void LimparSnapshotsFuturos(int etapa)
    {
        saveAtual.snapshotsPorPasso.RemoveAll(s => s.etapa > etapa);
    }

    public void CarregarSave()
    {
        for (int i = 0; i < saveAtual.etapas.Count && i < etapas.Length; i++)
        {
            var es = saveAtual.etapas[i];
            etapas[i].Desbloqueada         = es.desbloqueada;
            etapas[i].concluida            = es.concluida;
            etapas[i].ultimoPassoConcluido = es.ultimoPassoConcluido;
            for (int j = 0; j < es.passosConcluidos.Count && j < etapas[i].passos.Length; j++)
                etapas[i].passos[j].concluido = es.passosConcluidos[j];
        }

        etapaAtual = saveAtual.etapaAtual;
        passoAtual = saveAtual.passoAtual;

        StopAllCoroutines();
        StartCoroutine(InicarComDelay_Etapa(etapaAtual));
    }

    public void NovoJogo()
    {
        SaveSystem.Deletar();
        saveAtual = new SaveData();
        etapas[0].Desbloqueada = true;
        StartCoroutine(InicarComDelay());
    }

    IEnumerator InicarComDelay()
    {
        yield return new WaitForSeconds(0f);
        IniciarEtapa(0);
    }

    IEnumerator InicarComDelay_Etapa(int index)
    {
        yield return new WaitForSeconds(0.5f);
        IniciarEtapa(index);
    }

    public void IniciarEtapa(int index)
    {
        if (etapas == null || index < 0 || index >= etapas.Length) return;
        if (!etapas[index].Desbloqueada) return;

        StopAllCoroutines();
        CancelarFalaAtual();

        bool voltandoEtapa = index < etapaAtual;

        etapaAtual = index;

        if (Indicador != null)
            Indicador.PararEEsconder();

        minigameManager.DesativarTodos();
        minigameAtivo = false;

        DragItem.DescongelarTodos();

        if (voltandoEtapa)
        {
            LimparSnapshotsFuturos(index);
        }

        if (etapas[index].concluida)
        {
            passoAtual = 0;
            foreach (var p in etapas[index].passos)
                p.concluido = false;
            etapas[index].ultimoPassoConcluido = -1;
        }
        else
        {
            passoAtual = voltandoEtapa ? 0 : EncontrarPrimeiroPassoNaoConcluido(index);
        }

        bloquearInput = true;
        StartCoroutine(RestaurarEIniciar(index));
    }

    IEnumerator RestaurarEIniciar(int index)
    {
        var snap = SaveSystem.EncontrarSnapshot(saveAtual, index, 0);
        if (snap != null && snap.objetos.Count > 0)
        {
            RestaurarSnapshotDoPasso(index, 0);
        }
        else
        {
            GravarSnapshotDoPasso(index, 0);
        }

        yield return null;

        yield return TocarFalas(etapas[index].Falasintroducao, null);
        aguardandoFala = false;
        bloquearInput  = false;
        IniciarPasso();
    }

    int EncontrarPrimeiroPassoNaoConcluido(int indexEtapa)
    {
        var passos = etapas[indexEtapa].passos;
        for (int i = 0; i < passos.Length; i++)
        {
            if (!passos[i].concluido)
                return i;
        }
        return passos.Length - 1;
    }

    void IniciarPasso()
    {
        if (passoAtual < 0 || passoAtual >= etapas[etapaAtual].passos.Length)
            return;

        var passo = etapas[etapaAtual].passos[passoAtual];

        if (!passo.concluido && passo.salvarSnapshot)
        {
            SalvarSnapshot();
            GravarSnapshotDoPasso(etapaAtual, passoAtual);
        }

        minigameManager.DesativarTodos();
        minigameAtivo = false;

        DragItem.DescongelarTodos();

        if (Indicador != null && passo.objetoAlvo != null && passo.objetoAlvo)
        {
            Indicador.gameObject.SetActive(true);

            if (passo.tipoConclusao == TipoConclusao.ArrastarPara)
                Indicador.MostrarTrajeto(passo.objetoAlvo.transform, passo.destino, true);
            else
                Indicador.SeguirTransform(passo.objetoAlvo.transform);
        }

        bloquearInput  = false;
        aguardandoFala = false;

        if (passo.tipoConclusao == TipoConclusao.MiniGame && passo.minigame != null && passo.minigame)
            PrepararMinigame(passo);
        else
            minigameManager.DesativarTodos();

        falaAtualCoroutine = StartCoroutine(TocarFalas(passo.falas, null));
    }

    public void TentarPorColisao(GameObject objeto)
    {
        TentarConcluir(objeto, TipoConclusao.ArrastarPara);
    }

    public void TentarConcluirPorClique(GameObject clicado)
    {
        TentarConcluir(clicado, TipoConclusao.Clique);
    }

    public void TentarConcluirPorArraste(GameObject arrastado, GameObject destino)
    {
        TentarConcluirComDestino(arrastado, destino);
    }

    public void TentarConcluirComDestino(GameObject objetoA, GameObject objetoB)
    {
        if (etapaAtual < 0 || etapaAtual >= etapas.Length) return;
        var etapa = etapas[etapaAtual];
        if (passoAtual < 0 || passoAtual >= etapa.passos.Length) return;
        var passo = etapa.passos[passoAtual];

        if (objetoA == null || !objetoA) return;
        if (objetoB == null || !objetoB) return;

        if (passo.tipoConclusao != TipoConclusao.ArrastarPara)
            return;

        if (passo.objetoAlvo == null || !passo.objetoAlvo)
            return;

        if (passo.objetoAlvo != objetoA)
            return;

        if (passo.destino == null || !passo.destino)
            return;

        if (!objetoB.transform.IsChildOf(passo.destino))
            return;

        minigameManager.DesativarTodos();
        minigameAtivo = false;
        AvancarPasso();
    }

    public void TentarConcluir(GameObject objeto, TipoConclusao tipo)
    {
        if (bloquearInput) return;
        if (objeto == null || !objeto) return;

        var etapa = etapas[etapaAtual];
        if (passoAtual < 0 || passoAtual >= etapa.passos.Length) return;

        var passo = etapa.passos[passoAtual];
        if (passo.tipoConclusao != tipo) return;
        if (passo.objetoAlvo == null || !passo.objetoAlvo) return;
        if (passo.objetoAlvo != objeto) return;

        AvancarPasso();
    }

    void AvancarPasso()
    {
        CancelarFalaAtual();

        minigameAtivo = false;
        bloquearInput = false;

        var passo = etapas[etapaAtual].passos[passoAtual];
        etapas[etapaAtual].ultimoPassoConcluido = passoAtual;
        passo.concluido = true;

        if (Indicador != null)
            Indicador.PararEEsconder();

        passo.aoConcluir?.Invoke();
        SalvarEmDisco();

        if (passo.falasFinalizacao != null && passo.falasFinalizacao.Length > 0)
        {
            StartCoroutine(FinalizarPasso(passo));
            return;
        }

        passoAtual++;

        if (passoAtual >= etapas[etapaAtual].passos.Length)
            falaAtualCoroutine = StartCoroutine(FinalizarEtapa());
        else
            IniciarPasso();
    }

    public void SolicitarTrocaEtapa(int novaEtapa)
    {
        if (novaEtapa < 0 || novaEtapa >= etapas.Length) return;
        if (!etapas[novaEtapa].Desbloqueada) return;

        SalvarEmDisco();

        GravarSnapshotDoPasso(etapaAtual, passoAtual);

        ultimoSnapShot.etapa = etapaAtual;
        ultimoSnapShot.passo = passoAtual;

        TrocarEtapa(novaEtapa);
    }

    public void SalvarSnapshot()
    {
        ultimoSnapShot.etapa            = etapaAtual;
        ultimoSnapShot.passo            = passoAtual;
        ultimoSnapShot.estavaEmMinigame = minigameAtivo;

        var objetos = FindObjectsOfType<Resetavel>(true);
        foreach (var obj in objetos)
            obj.SalvarSnapshot();
    }

    void TrocarEtapa(int index)
    {
        StopAllCoroutines();
        IniciarEtapa(index);
    }

    public void VoltarParaOndeParou()
    {
        RestaurarSnapshot();
    }

    public void RestaurarSnapshot()
    {
        CancelarFalaAtual();
        StopAllCoroutines();

        minigameManager.DesativarTodos();
        minigameAtivo = false;
        bloquearInput = false;

        DragItem.DescongelarTodos();

        etapaAtual = ultimoSnapShot.etapa;
        passoAtual = ultimoSnapShot.passo;

        ResetarObjetosCena();

        StartCoroutine(IniciarPassoNoProximoFrame());
    }

    IEnumerator IniciarPassoNoProximoFrame()
    {
        yield return new WaitForFixedUpdate();
        yield return null;
        IniciarPasso();
    }

    IEnumerator FinalizarEtapa()
    {
        bloquearInput  = true;
        aguardandoFala = true;

        var etapa = etapas[etapaAtual];
        yield return TocarFalas(etapa.falasFinalizacao, null);

        etapa.concluida = true;

        GravarSnapshotDoPasso(etapaAtual, passoAtual);

        int proximaEtapa = etapaAtual + 1;
        if (proximaEtapa < etapas.Length)
        {
            etapas[proximaEtapa].Desbloqueada = true;
            SalvarEmDisco();
        }

        aguardandoFala = false;
        bloquearInput = false;

        if (proximaEtapa < etapas.Length)
            IniciarEtapa(proximaEtapa);
    }

    IEnumerator FinalizarPasso(PassoAlt passo)
    {
        bloquearInput  = true;
        aguardandoFala = true;

        yield return TocarFalas(passo.falasFinalizacao, null);

        aguardandoFala = false;
        bloquearInput  = false;

        passoAtual++;

        if (passoAtual >= etapas[etapaAtual].passos.Length)
            StartCoroutine(FinalizarEtapa());
        else
            IniciarPasso();
    }

    IEnumerator TocarFalas(Fala[] falas, System.Action aoFinalizar)
    {
        aguardandoFala = true;

        if (falas != null && falas.Length > 0)
        {
            foreach (var fala in falas)
            {
                if (textUI != null)
                    textUI.text = fala.texto;

                if (fala.audio != null)
                {
                    audioSource.clip = fala.audio;
                    audioSource.Play();

                    float tempo = fala.audio.length;
                    float t = 0f;
                    while (t < tempo)
                    {
                        if (!aguardandoFala) yield break;
                        t += Time.deltaTime;
                        yield return null;
                    }
                }
                else
                {
                    float t = 0f;
                    while (t < 2f)
                    {
                        if (!aguardandoFala) yield break;
                        t += Time.deltaTime;
                        yield return null;
                    }
                }
            }
        }

        aguardandoFala = false;
        aoFinalizar?.Invoke();
    }

    public bool PassoAtivoNaEtapa(int indexEtapa)
    {
        return etapaAtual == indexEtapa &&
               passoAtual >= 0 &&
               passoAtual < etapas[etapaAtual].passos.Length;
    }

    void ResetarObjetosCena()
    {
        var objetos = FindObjectsOfType<Resetavel>(true);
        foreach (var obj in objetos)
            obj.Resetar();
    }

    void PrepararMinigame(PassoAlt passo)
    {
        minigameAtivo = true;

        DragItem.CongelarTodos();

        passo.minigame.Resetar();
        passo.minigame.onConcluido.RemoveAllListeners();
        passo.minigame.onConcluido.AddListener(() =>
        {
            minigameAtivo = false;
            minigameManager.DesativarTodos();

            DragItem.DescongelarTodos();

            AvancarPasso();
        });

        minigameManager.Ativar(passo.minigame);
    }

    void CancelarFalaAtual()
    {
        aguardandoFala = false;

        if (falaAtualCoroutine != null)
        {
            StopCoroutine(falaAtualCoroutine);
            falaAtualCoroutine = null;
        }

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        if (textUI != null)
            textUI.text = "";
    }

    public void ResetarEtapaParaInicial(int index)
    {
        var etapa = etapas[index];
        etapa.concluida = false;
        etapa.ultimoPassoConcluido = -1;
        foreach (var p in etapa.passos)
            p.concluido = false;

        var resetaveis = FindObjectsOfType<Resetavel>(true);
        foreach (var r in resetaveis)
            r.ResetarParaInicial();

        SalvarEmDisco();
    }

    [ContextMenu("Deletar Save")]
    public void DeletarSave()
    {
        SaveSystem.Deletar();
        saveAtual = new SaveData();
    }
}

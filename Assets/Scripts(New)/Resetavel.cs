using UnityEngine;

/// <summary>
/// Adicione este componente em qualquer objeto que precisa ter sua
/// posição/rotação/escala salva e restaurada pelo sistema de save.
/// 
/// O ID é gerado automaticamente pelo nome do objeto na hierarquia.
/// Se dois objetos tiverem o mesmo nome, defina um ID manual no Inspector.
/// </summary>
public class Resetavel : MonoBehaviour
{
    [Tooltip("ID único para o save. Deixe vazio para gerar automaticamente pelo caminho na hierarquia.")]
    public string idManual = "";

    [Tooltip("Se marcado, este objeto NÃO será salvo/restaurado pelo sistema de snapshot.")]
    public bool ignorarSnapshot = false;

    // ─── Estado inicial (capturado no Awake) ───
    private Vector3 posInicial;
    private Quaternion rotInicial;
    private Vector3 escalaInicial;
    private bool ativoInicial;

    // ─── Snapshot em memória (usado pelo VoltarParaOndeParou) ───
    private Vector3 posSnapshot;
    private Quaternion rotSnapshot;
    private Vector3 escalaSnapshot;
    private bool ativoSnapshot;
    private bool temSnapshot = false;

    void Awake()
    {
        posInicial    = transform.localPosition;
        rotInicial    = transform.localRotation;
        escalaInicial = transform.localScale;
        ativoInicial  = gameObject.activeSelf;
    }

    // ─────────────────────────────────────────
    //  ID único
    // ─────────────────────────────────────────

    public string ObterID()
    {
        if (!string.IsNullOrEmpty(idManual))
            return idManual;

        string caminho = gameObject.name;
        Transform pai = transform.parent;
        while (pai != null)
        {
            caminho = pai.name + "/" + caminho;
            pai = pai.parent;
        }
        return caminho;
    }

    // ─────────────────────────────────────────
    //  Snapshot em memória
    // ─────────────────────────────────────────

    public void SalvarSnapshot()
    {
        posSnapshot    = transform.localPosition;
        rotSnapshot    = transform.localRotation;
        escalaSnapshot = transform.localScale;
        ativoSnapshot  = gameObject.activeSelf;
        temSnapshot    = true;
    }

    public void Resetar()
    {
        if (temSnapshot)
        {
            transform.localPosition = posSnapshot;
            transform.localRotation = rotSnapshot;
            transform.localScale    = escalaSnapshot;
            gameObject.SetActive(ativoSnapshot);
        }
        else
        {
            transform.localPosition = posInicial;
            transform.localRotation = rotInicial;
            transform.localScale    = escalaInicial;
            gameObject.SetActive(ativoInicial);
        }
    }

    // ─────────────────────────────────────────
    //  Para o SaveSystem em disco
    // ─────────────────────────────────────────

    /// <summary>Exporta o estado local atual para ser salvo no JSON.</summary>
    public ObjetoSalvo ExportarParaSave()
    {
        if (ignorarSnapshot) return null;

        Vector3 p    = transform.localPosition;
        Quaternion r = transform.localRotation;
        Vector3 s    = transform.localScale;

        return new ObjetoSalvo
        {
            id    = ObterID(),
            px = p.x, py = p.y, pz = p.z,
            rx = r.x, ry = r.y, rz = r.z, rw = r.w,
            sx = s.x, sy = s.y, sz = s.z,
            ativo = gameObject.activeSelf // salva estado ativo/inativo
        };
    }

    /// <summary>Aplica um estado carregado do JSON, incluindo ativo/inativo.</summary>
    public void AplicarDoSave(ObjetoSalvo dado)
    {
        if (ignorarSnapshot) return;

        transform.localPosition = new Vector3(dado.px, dado.py, dado.pz);
        transform.localRotation = new Quaternion(dado.rx, dado.ry, dado.rz, dado.rw);
        transform.localScale    = new Vector3(dado.sx, dado.sy, dado.sz);

        // Restaura estado ativo/inativo exatamente como estava no snapshot
        gameObject.SetActive(dado.ativo);

        SalvarSnapshot();
    }

    /// <summary>Reseta para o estado inicial absoluto (ignora snapshot).</summary>
    public void ResetarParaInicial()
    {
        transform.localPosition = posInicial;
        transform.localRotation = rotInicial;
        transform.localScale    = escalaInicial;
        gameObject.SetActive(ativoInicial);
        temSnapshot = false;
    }
}
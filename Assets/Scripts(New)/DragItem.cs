using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragItem : MonoBehaviour
{
    [SerializeField] private Camera cam;
    public Rigidbody2D rb2;

    [Header("Margem extra fora da tela antes de considerar 'fora da câmera'")]
    public float margemForaCamera = 0.1f;

    [Header("Limite de arraste (opcional)")]
    [Tooltip("Se definido, o item não pode ser arrastado para fora desta área (ex: a área visível da tela/mesa).")]
    public BoxCollider2D areaLimite;

    private Vector3 offset;
    private bool isDragging;
    private Vector3 posicaoNaPia;
    private Quaternion rotacaoNaPia;
    private Vector3 ultimaPosicaoColisao;

    // Guarda o destino que está em contato AGORA
    private GameObject destinoAtual = null;

    // Controle de congelamento (minigame ativo)
    private bool congelado = false;
    private Collider2D col2d;

    // ─── Lista estática de todos os DragItems vivos, pra ignorar colisão entre eles ───
    private static readonly List<DragItem> todosOsItens = new List<DragItem>();

    // ─── Congelar / Descongelar todos os DragItems da cena ──────────
    public static void CongelarTodos()
    {
        foreach (var item in todosOsItens)
        {
            if (item != null)
                item.Congelar();
        }
    }

    public static void DescongelarTodos()
    {
        foreach (var item in todosOsItens)
        {
            if (item != null)
                item.Descongelar();
        }
    }

    public void Congelar()
    {
        congelado = true;

        // Para o arraste imediatamente
        isDragging = false;
        destinoAtual = null;

        if (rb2 != null)
        {
            rb2.bodyType = RigidbodyType2D.Dynamic;
            rb2.velocity = Vector2.zero;
            rb2.angularVelocity = 0f;
            rb2.gravityScale = 0f;
        }
    }

    public void Descongelar()
    {
        congelado = false;

        if (rb2 != null)
            rb2.gravityScale = 1f;

        if (col2d != null)
            col2d.enabled = true;
    }

    // Chamado ao restaurar snapshot: limpa física sem alterar posição
    public void ResetarFisica()
    {
        congelado = false;
        isDragging = false;
        destinoAtual = null;

        if (rb2 != null)
        {
            rb2.bodyType = RigidbodyType2D.Dynamic;
            rb2.velocity = Vector2.zero;
            rb2.angularVelocity = 0f;
            rb2.gravityScale = 1f;
            rb2.freezeRotation = true;
        }

        if (col2d != null)
            col2d.enabled = true;
    }

    // ────────────────────────────────────────────────────────────────

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        posicaoNaPia = transform.position;
        rotacaoNaPia = transform.rotation;
        ultimaPosicaoColisao = transform.position;

        col2d = GetComponent<Collider2D>();

        // Garante que o DragItem tem Resetavel para o snapshot
        if (GetComponent<Resetavel>() == null)
            gameObject.AddComponent<Resetavel>();

        // Garante que o objeto fica na frente de outros sprites (ex: sujeira)
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = 15;

        if (rb2 != null)
        {
            rb2.gravityScale = 1f;
            rb2.freezeRotation = true;
        }
    }

    void OnEnable()
    {
        isDragging = false;
        destinoAtual = null;
        congelado = false;

        // Redefine sortingOrder ao reativar
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = 15;

        if (rb2 != null)
        {
            rb2.bodyType = RigidbodyType2D.Dynamic;
            rb2.velocity = Vector2.zero;
            rb2.gravityScale = 1f;
        }

        if (col2d != null)
            col2d.enabled = true;

        // ─── Ignora colisão física com todos os outros DragItems já existentes ───
        foreach (var outro in todosOsItens)
        {
            if (outro != null && outro != this && outro.col2d != null && col2d != null)
                Physics2D.IgnoreCollision(col2d, outro.col2d, true);
        }

        if (!todosOsItens.Contains(this))
            todosOsItens.Add(this);
    }

    void OnDisable()
    {
        isDragging = false;
        destinoAtual = null;

        todosOsItens.Remove(this);
    }

    // ─── Detecta quando ENTRA num destino ───────────────────────────
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Destino"))
            destinoAtual = other.gameObject;
    }

    // ─── Limpa se SAIR do destino sem soltar ────────────────────────
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Destino") && destinoAtual == other.gameObject)
            destinoAtual = null;
    }

    // ─── Registra posição da colisão ────────────────────────────────
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDragging)
            ultimaPosicaoColisao = transform.position;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!isDragging)
            ultimaPosicaoColisao = transform.position;
    }

    // ────────────────────────────────────────────────────────────────

    void OnMouseDown()
    {
        // Bloqueia interação enquanto congelado
        if (congelado) return;

        if (cam == null) return;

        offset = transform.position - GetMouseWorld();
        isDragging = true;

        if (rb2 != null)
        {
            rb2.velocity = Vector2.zero;
            rb2.angularVelocity = 0f;
            rb2.gravityScale = 0f;

            // Evita que colisões/triggers vizinhos travem o arraste
            rb2.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    void Update()
    {
        if (congelado) return;

        if (isDragging)
        {
            if (InputHelper.GetButtonUp())
            {
                isDragging = false;
                AoSoltar();
                return;
            }

            Vector3 alvo = GetMouseWorld() + offset;

            // Trava o alvo dentro da área limite, se definida
            alvo = ClampNaArea(alvo);

            if (rb2 != null)
                rb2.MovePosition(alvo);
            else
                transform.position = alvo;
        }
    }

    void FixedUpdate()
    {
        if (isDragging) return;
        if (congelado) return;

        // Não está sendo arrastado: se caiu (por física) fora da câmera, volta pra colisão.
        if (EstaForaDaCamera())
            VoltarParaPia();
    }

    void AoSoltar()
    {
        if (rb2 != null)
        {
            rb2.bodyType = RigidbodyType2D.Dynamic;
            rb2.gravityScale = 1f;
            rb2.velocity = Vector2.zero;
            rb2.angularVelocity = 0f;

            // Garante que nenhuma velocidade residual do arraste seja aplicada
            StartCoroutine(ZerarVelocidadeProximoFrame());
        }

        // Conclui se estiver DENTRO de um destino no momento de soltar
        if (destinoAtual != null)
        {
            GameObject destinoFinal = destinoAtual;
            destinoAtual = null;

            Gamemaneger.instance.TentarConcluirComDestino(gameObject, destinoFinal);
            return;
        }

        // Se foi solto fora da área visível, volta pra posição da colisão.
        if (EstaForaDaCamera())
            VoltarParaPia();
        else
            ultimaPosicaoColisao = transform.position;
    }

    IEnumerator ZerarVelocidadeProximoFrame()
    {
        yield return new WaitForFixedUpdate();
        if (rb2 != null)
        {
            rb2.velocity = Vector2.zero;
            rb2.angularVelocity = 0f;
        }
    }

    bool EstaForaDaCamera()
    {
        if (cam == null) return false;

        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);

        return viewportPos.x < -margemForaCamera || viewportPos.x > 1f + margemForaCamera ||
               viewportPos.y < -margemForaCamera || viewportPos.y > 1f + margemForaCamera;
    }

    Vector3 GetMouseWorld()
    {
        Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;
        return pos;
    }

    // ─── Restringe a posição alvo aos limites do BoxCollider2D "areaLimite" ───
    Vector3 ClampNaArea(Vector3 alvo)
    {
        if (areaLimite == null)
            return alvo;

        Bounds b = areaLimite.bounds;

        alvo.x = Mathf.Clamp(alvo.x, b.min.x, b.max.x);
        alvo.y = Mathf.Clamp(alvo.y, b.min.y, b.max.y);

        return alvo;
    }

    void VoltarParaPia()
    {
        if (rb2 != null)
        {
            rb2.bodyType = RigidbodyType2D.Dynamic;
            rb2.velocity = Vector2.zero;
            rb2.angularVelocity = 0f;
            rb2.gravityScale = 0f;
            rb2.freezeRotation = true;
            rb2.position = ultimaPosicaoColisao;
            rb2.rotation = rotacaoNaPia.eulerAngles.z;
        }

        transform.position = ultimaPosicaoColisao;
        transform.rotation = rotacaoNaPia;
    }
}
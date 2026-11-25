using UnityEngine;

public class CopoController : MonoBehaviour
{
    public float velocidade = 10f;
    private Rigidbody2D rb;

    private Collider2D cd2;

    private bool segurando;
    private Vector3 mousePos;

    [Header("Limite de movimento (opcional)")]
    [Tooltip("Se definido, o copo não pode sair dos limites X deste collider.")]
    public BoxCollider2D areaLimite;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cd2 = GetComponent<Collider2D>();
    }

    void OnDisable()
    {
        segurando = false;
    }
    void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        // Só começa a segurar se clicou em cima do copo
        Vector2 clique = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (cd2.OverlapPoint(clique))
            segurando = true;
    }
    else if (Input.GetMouseButtonUp(0))
        segurando = false;

    if (!Input.GetMouseButton(0))
        segurando = false;

    if (segurando)
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
}
    void FixedUpdate()
    {
        if (!segurando) return;

        float alvoX = mousePos.x;

        // Trava o X dentro da área limite, se definida
        if (areaLimite != null)
        {
            Bounds b = areaLimite.bounds;
            alvoX = Mathf.Clamp(alvoX, b.min.x, b.max.x);
        }

        Vector2 novaPos = new Vector2(alvoX, rb.position.y);
        rb.MovePosition(Vector2.Lerp(rb.position, novaPos, velocidade * Time.fixedDeltaTime));
    }
}
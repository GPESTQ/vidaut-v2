using UnityEngine;

public class MinigameAbrirPasta : MiniGameBase
{
    [Header("Referências")]
    [SerializeField] private Transform lidTransform;
    [SerializeField] private SpriteRenderer lidRenderer;
    [SerializeField] private Sprite[] frames;

    [Header("Config")]
    [SerializeField] private int girosNecessarios = 4;
    [SerializeField] private float alturaMaxima = 1f;
    [SerializeField] private float sensibilidade = 0.01f;
    [SerializeField] private float amortecimento = 5f;
    [SerializeField] private float peso = 2f;

    private float _velocidade = 0f;
    private float _alturaAtual = 0f;
    private int _giros = 0;
    private Vector3 _posInicial;
    private Vector3 _lastMousePos;

    void Awake()
    {
        if (lidTransform != null)
            _posInicial = lidTransform.localPosition;
    }

    public override void Iniciar()
    {
        base.Iniciar();

        _alturaAtual = 0f;
        _giros = 0;
        _velocidade = 0f;

        AplicarEstadoVisual();
    }

    public override void Resetar()
    {
        base.Resetar();

        _alturaAtual = 0f;
        _giros = 0;
        _velocidade = 0f;

        AplicarEstadoVisual();
    }

    void Update()
    {
        if (!iniciado || concluido) return;

        HandleInput();

        _alturaAtual += _velocidade * Time.deltaTime;
        _alturaAtual = Mathf.Clamp01(_alturaAtual);

        _velocidade = Mathf.Lerp(_velocidade, 0f, (amortecimento + peso) * Time.deltaTime);

        AplicarEstadoVisual();

        if (_alturaAtual >= 1f)
            Concluir();
    }

    private void HandleInput()
    {
        if (InputHelper.GetButtonDown())
            _lastMousePos = InputHelper.GetPosition();

        if (InputHelper.GetButton())
        {
            float delta = InputHelper.GetPosition().x - _lastMousePos.x;
            _lastMousePos = InputHelper.GetPosition();
            _velocidade += delta * sensibilidade;
        }
    }

    private void AplicarEstadoVisual()
    {
        if (lidTransform != null)
            lidTransform.localPosition = _posInicial + new Vector3(0f, _alturaAtual * alturaMaxima, 0f);

        if (lidRenderer != null && frames.Length > 0)
        {
            int index = Mathf.FloorToInt(_alturaAtual * (frames.Length - 1));
            index = Mathf.Clamp(index, 0, frames.Length - 1);
            lidRenderer.sprite = frames[index];
        }
    }
}

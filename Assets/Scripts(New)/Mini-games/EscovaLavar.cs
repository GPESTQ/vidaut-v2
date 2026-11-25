using UnityEngine;
using UnityEngine.UI;

public class EscovaLavar : MiniGameBase
{
    [Header("Refer�ncias")]
    public Transform escova;
    public Collider2D areaPia;
    public Image barraTempo;

    [Header("Configura��es")]
    public float tempoNecessario = 3f;
    public float velocidadeRetorno = 1.5f;

    float tempoAtual;
    bool arrastando;
    bool dentroDaPia;
    Vector3 offsetMouse;
    Vector3 posicaoInicial;

    void Awake()
    {
        posicaoInicial = escova.position;
    }

    public override void Iniciar()
    {
        base.Iniciar();
        tempoAtual = 0f;
        barraTempo.fillAmount = 0f;
        escova.position = posicaoInicial;
        arrastando = false;
    }

    void Update()
    {
        HandleDrag();
        AtualizarTempo();
    }

    void HandleDrag()
    {
        if (InputHelper.GetButtonDown())
        {
            Vector3 mouseWorld = InputHelper.GetWorldPosition(Camera.main);

            var col = escova.GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(mouseWorld))
            {
                arrastando = true;
                offsetMouse = escova.position - mouseWorld;
            }
        }

        if (arrastando && InputHelper.GetButton())
        {
            Vector3 mouseWorld = InputHelper.GetWorldPosition(Camera.main);
            escova.position = mouseWorld + offsetMouse;
        }

        if (InputHelper.GetButtonUp())
        {
            arrastando = false;

            if (tempoAtual >= tempoNecessario)
            {
                Concluir();
            }
        }
    }

    void AtualizarTempo()
    {
        if (!arrastando) return;

        dentroDaPia = areaPia.OverlapPoint(escova.position);

        if (dentroDaPia)
        {
            tempoAtual += Time.deltaTime;
        }
        else
        {
            // Decaimento suave (melhor para TEA)
            tempoAtual = Mathf.Max(tempoAtual - Time.deltaTime * velocidadeRetorno, 0);
        }

        barraTempo.fillAmount = tempoAtual / tempoNecessario;
    }
}

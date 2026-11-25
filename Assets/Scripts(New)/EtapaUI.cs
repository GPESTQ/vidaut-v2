using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EtapaUI : MonoBehaviour
{
    public int index;
    public Button btn;
    public GameObject EstadoAtivo;      // destaca a etapa atual
    public GameObject EstadoConcluido;  // mostra que a etapa foi concluída

    void Update()
    {
        Atualizar();
    }

    void Atualizar()
    {
        var gm = Gamemaneger.instance;
        if (gm == null || gm.etapas == null) return;
        if (index < 0 || index >= gm.etapas.Length) return;

        var etapa = gm.etapas[index];

        btn.interactable = etapa.Desbloqueada;

        EstadoConcluido.SetActive(etapa.concluida);
        EstadoAtivo.SetActive(gm.etapaAtual == index);

        var img = EstadoAtivo.GetComponent<Image>();
        if (img != null)
            img.enabled = gm.etapaAtual == index;
    }

    public void OnClickEtapa(int etapaIndex)
    {
        var gm = Gamemaneger.instance;
        if (gm == null) return;

        gm.SolicitarTrocaEtapa(etapaIndex);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoClicavel : MonoBehaviour
{
    public TipoConclusao TC;


    void OnMouseDown()
    {
        Gamemaneger.instance.TentarConcluir(gameObject, TipoConclusao.Clique);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Gamemaneger.instance.TentarConcluir(gameObject, TipoConclusao.Colisao);
    }

    void OnTriggerEnter2D(Collider2D other)
    {   
    if(!other.CompareTag("Destino")) return;
    
    Gamemaneger.instance.TentarConcluirComDestino(gameObject, other.gameObject);
    }


}

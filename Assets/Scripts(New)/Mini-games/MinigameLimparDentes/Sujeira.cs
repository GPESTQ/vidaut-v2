// Sujeira.cs
using System;
using UnityEngine;

public class Sujeira : MonoBehaviour
{
    public static Action onSujeiraRemovida;

    private bool limpa;

    void Awake()
    {
        DefinirSortingOrder();
    }

    void OnEnable()
    {
        DefinirSortingOrder();
    }

    void DefinirSortingOrder()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = 8;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (limpa) return;

        if (other.CompareTag("Escova"))
        {
            limpa = true;
            onSujeiraRemovida?.Invoke();
            // desativa em vez de destruir imediatamente
            // evita NullReference em outros scripts que ainda seguram referência
            gameObject.SetActive(false);
            Destroy(gameObject, 0.1f);
        }
    }
}
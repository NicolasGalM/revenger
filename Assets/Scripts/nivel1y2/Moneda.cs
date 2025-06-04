using UnityEngine;

public class Moneda : MonoBehaviour
{
    public int valor = 1;
    public GM gameManager;
    public AudioClip sonidoRecoger; // <- Añadido

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Reproduce el sonido en la posición de la moneda
            AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position);

            // Suma puntos y destruye la moneda
            gameManager.SumarPuntos(valor);
            Destroy(gameObject);
        }
    }
}

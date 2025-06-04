using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject efecto;
    [SerializeField] private AudioClip sonidoMuerte; // <- Añadido
    private Animator animator;

    public bool fuePisado = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.GetContact(0).normal.y <= -0.9f)
            {
                fuePisado = true;
                animator.SetTrigger("Golpe");
                other.gameObject.GetComponent<PLController>().Rebote();
            }
        }
    }

    public void Golpe()
    {
        // Reproducir sonido
        AudioSource.PlayClipAtPoint(sonidoMuerte, transform.position);

        // Efecto visual
        Instantiate(efecto, transform.position, transform.rotation);

        // Reset de otros enemigos
        Enemy[] enemigos = FindObjectsOfType<Enemy>();
        foreach (Enemy enemigo in enemigos)
        {
            if (enemigo != this)
            {
                enemigo.ResetPisado();
            }
        }

        // Eliminar enemigo
        Destroy(gameObject);
    }

    public void ResetPisado()
    {
        fuePisado = false;
    }
}

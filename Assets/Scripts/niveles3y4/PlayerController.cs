using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocidad;
    public float fuerzaSalto;
    public float saltosMaximos;
    public float fuerzaGolpe;
    public LayerMask capaSuelo;
    public AudioClip audioClip;

    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private bool mirandoDerecha = true;
    private float saltosRestantes;
    private Animator animator;
    private bool puedeMoverse = true;

    [Header("Knockback")]
    public float fuerzaGolpeX = 10f;   // fuerza en X
    public float fuerzaGolpeY = 4f;    // fuerza en Y


    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        saltosRestantes = saltosMaximos;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        ProcesarMovimiento();
        ProcesarSalto();
    }

    private bool EstaEnSuelo()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.down,
            0.2f,
            capaSuelo
        );
        return raycastHit.collider != null;
    }

    private void ProcesarSalto()
    {
        if (EstaEnSuelo())
        {
            saltosRestantes = saltosMaximos;
        }

        if (Input.GetKeyDown(KeyCode.Space) && saltosRestantes > 0)
        {
            saltosRestantes--;
            // Reiniciamos la componente Y antes de aplicar el impulso
            rigidBody.linearVelocity = new Vector2(
                rigidBody.linearVelocity.x,
                0f
            );
            rigidBody.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            AudioManager.Instance.ReproducirSonido(audioClip);
        }
    }

    private void ProcesarMovimiento()
    {
        float inputMovimiento = Input.GetAxis("Horizontal");

        if (inputMovimiento != 0f)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }

        // Movemos en X manteniendo la velocidad Y actual
        rigidBody.linearVelocity = new Vector2(
            inputMovimiento * velocidad,
            rigidBody.linearVelocity.y
        );

        GestionarOrientacion(inputMovimiento);
    }

    private void GestionarOrientacion(float inputMovimiento)
    {
        // Si cambiamos de dirección, volteamos el sprite
        if ((mirandoDerecha && inputMovimiento < 0) ||
            (!mirandoDerecha && inputMovimiento > 0))
        {
            mirandoDerecha = !mirandoDerecha;
            transform.localScale = new Vector2(
                -transform.localScale.x,
                transform.localScale.y
            );
        }
    }

    public void AplicarGolpe()
    {
        puedeMoverse = false;

        // Determinar signo horizontal según la velocidad actual
        float signoX = (rigidBody.linearVelocity.x > 0f) ? -1f : 1f;

        // Construir el vector de knockback:
        // - X: fuerza completa en horizontal
        // - Y: fuerza completa en vertical (puedes ajustar knockbackY)
        Vector2 golpe = new Vector2(signoX * fuerzaGolpeX,
                                    fuerzaGolpeY);

        // Aplicar el impulso de golpe
        rigidBody.AddForce(golpe, ForceMode2D.Impulse);

        StartCoroutine(EsperarYActivarMovimiento());
    }

    IEnumerator EsperarYActivarMovimiento()
    {
        // Esperamos antes de comprobar si esta en el suelo.
        yield return new WaitForSeconds(0.1f);

        while (!EstaEnSuelo())
        {
            // Esperamos al siguiente frame.
            yield return null;
        }

        // Si ya está en suelo activamos el movimiento.
        puedeMoverse = true;
    }
}
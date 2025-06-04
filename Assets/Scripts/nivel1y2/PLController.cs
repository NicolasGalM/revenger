using UnityEngine;

public class PLController : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true;
    public AudioClip deathSound;     
    private AudioSource audioSource;

    private bool haPerdidoVida = false; // 🔹 Controla que solo se pierda una vida por intento

    [Header("Rebote")]
    [SerializeField] private float velocidadRebote;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        haPerdidoVida = false; // 🔹 Se resetea al iniciar la escena
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * 5f, rb.linearVelocity.y); // 🔹 Corrección de "linearVelocity" a "velocity"

        if (move > 0 && !facingRight)
        {
            Flip();
        }
        else if (move < 0 && facingRight)
        {
            Flip();
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 8f);
        }

        if (transform.position.y < -10f)
        {
            Die();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if (collision.gameObject.CompareTag("EnemyFire") && !haPerdidoVida)
        {
            Enemy enemigo = collision.gameObject.GetComponent<Enemy>();
            if(enemigo != null && !enemigo.fuePisado){
                haPerdidoVida = true;
                Die();
            }
        }

        /*if (collision.gameObject.CompareTag("End"))
        {
            Debug.Log("Jugador tocó End. Volviendo a la pantalla inicial...");

            if (GM.instance != null)
            {
                GM.instance.CambiarEscena("IntroScene");
            }
        }*/
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x = Mathf.Abs(theScale.x) * (facingRight ? 1 : -1); 
        transform.localScale = theScale;
    }

    public void Rebote(){
        rb.linearVelocity= new Vector2(rb.linearVelocity.x, velocidadRebote);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    void Die()
    {
        if (audioSource && deathSound)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (GM.instance != null)
        {
            GM.instance.ReduceVidas();

            if (GM.instance.vidas <= 0)
            {
                return;
            }

            Debug.Log("Te quedan " + GM.instance.vidas + " vidas");
        }

        Invoke("RestartLevel", 2f);
    }

    void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}

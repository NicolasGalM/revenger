using UnityEngine;

public class PLController : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true;
    public AudioClip deathSound;
    [Header("Sonidos")]
    public AudioClip toqueEnemySound; // Sonido para cada toque con EnemyFire
    private AudioSource audioSource;
    private int golpesRecibidos = 0;
    private bool haPerdidoVida = false; // Para evitar daño múltiple sin invencibilidad
    
    [Header("Sistema de Toques")]
    [SerializeField] private int toquesEnemyFire = 0; // Contador de toques con EnemyFire
    [SerializeField] private int toquesNecesarios = 2; // Toques necesarios para recibir daño
    
    [Header("Rebote")]
    [SerializeField] private float velocidadRebote;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        haPerdidoVida = false;
        toquesEnemyFire = 0;
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * 5f, rb.linearVelocity.y);

        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();

        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 8f);

        if (transform.position.y < -10f)
            Morir();

        // Reseteamos para poder perder vida otra vez después de la invencibilidad
        if (GM.instance != null && !GM.instance.invencible && haPerdidoVida)
        {
            haPerdidoVida = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if (collision.gameObject.CompareTag("EnemyFire"))
        {
            Enemy enemigo = collision.gameObject.GetComponent<Enemy>();
            if (enemigo == null) return;

            // Detecta contacto desde arriba (normal.y > 0.5f)
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    if (!enemigo.fuePisado)
                    {
                        enemigo.fuePisado = true;
                        Destroy(collision.gameObject); // Mata enemigo
                        Rebote();
                    }
                    return; // No pierdes vida ni invencibilidad
                }
            }

            // Daño frontal, solo si no estás invencible y no perdiste vida ya
            if (GM.instance != null && !GM.instance.invencible && !haPerdidoVida)
            {
                // Incrementar contador de toques
                toquesEnemyFire++;
                Debug.Log($"🔥 Toque con EnemyFire #{toquesEnemyFire}/{toquesNecesarios}");
                
                // Reproducir sonido de toque (diferente al de muerte)
                if (audioSource && toqueEnemySound)
                    audioSource.PlayOneShot(toqueEnemySound);
                
                // Actualizar HUD con los toques actuales
                if (GM.instance != null)
                    GM.instance.ActualizarToques(toquesEnemyFire);
                
                // Verificar si se han alcanzado los toques necesarios
                if (toquesEnemyFire >= toquesNecesarios)
                {
                    haPerdidoVida = true;
                    GM.instance.RecibirDaño();
                    
                    // Reiniciar contador de toques
                    toquesEnemyFire = 0;
                    Debug.Log("🔥 Toques reiniciados a 0 después de recibir daño");
                    
                    // Actualizar HUD
                    GM.instance.ActualizarToques(toquesEnemyFire);

                    // Sonido de muerte (diferente al de toque)
                    if (audioSource && deathSound)
                        audioSource.PlayOneShot(deathSound);
                }
            }
        }
        
        if (collision.gameObject.CompareTag("Pinchos"))
        {
            if (GM.instance != null && !GM.instance.invencible && !haPerdidoVida)
            {
                haPerdidoVida = true;
                GM.instance.RecibirDaño();

                // Solo sonido o efectos, NO reiniciar nivel ni restar vida aquí
                if (audioSource && deathSound)
                    audioSource.PlayOneShot(deathSound);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x = Mathf.Abs(theScale.x) * (facingRight ? 1 : -1);
        transform.localScale = theScale;
    }

    public void Rebote()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, velocidadRebote);
    }

    void Morir()
    {
        if (GM.instance != null)
        {
            GM.instance.CaidaAlVacio();
        }
    }
    
    // Método público para obtener los toques actuales (útil para debugging o UI)
    public int GetToquesEnemyFire()
    {
        return toquesEnemyFire;
    }
    
    // Método para reiniciar toques externamente si es necesario
    public void ReiniciarToques()
    {
        toquesEnemyFire = 0;
        if (GM.instance != null)
            GM.instance.ActualizarToques(toquesEnemyFire);
        Debug.Log("🔥 Toques EnemyFire reiniciados externamente");
    }
}
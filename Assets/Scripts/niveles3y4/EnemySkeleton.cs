using UnityEngine;

public class EnemySkeleton : MonoBehaviour
{
    [Header("Patrulla genérica")]
    public Transform[] patrolPoints;      // Lista de puntos: A, B, C, ...
    public float patrolSpeed = 2f;

    [Header("Persecución")]
    public float chaseSpeed = 3.5f;
    public float chaseRadius = 5f;

    [Header("Ataque")]
    public float cooldownAtaque = 1.5f;
    private bool puedeAtacar = true;
    private SpriteRenderer spriteRenderer;

    private int currentIndex = 0;
    private Transform player;
    private SpriteRenderer sr;  // para el flipX

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        sr = GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Asegúrate de tener al menos 2 puntos en patrolPoints
    }

    void Update()
    {
        float distPlayer = Vector2.Distance(transform.position, player.position);

        if (distPlayer <= chaseRadius)
            ChasePlayer();
        else
            Patrol();

        FlipSprite();
    }

    void Patrol()
    {
        if (patrolPoints.Length < 2) return;

        Vector3 targetPos = patrolPoints[currentIndex].position;
        transform.position = Vector2.MoveTowards(transform.position,
                                                 targetPos,
                                                 patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
            currentIndex = (currentIndex + 1) % patrolPoints.Length;
    }

    void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position,
                                                 player.position,
                                                 chaseSpeed * Time.deltaTime);
    }

    void FlipSprite()
    {
        Vector2 dir = (Vector2)((Vector2.Distance(transform.position, player.position) <= chaseRadius)
                                 ? (player.position - transform.position)
                                 : (patrolPoints[currentIndex].position - transform.position));

        if (dir.x > 0.1f) sr.flipX = false;
        else if (dir.x < -0.1f) sr.flipX = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!puedeAtacar) return;

        if (other.gameObject.CompareTag("Player"))
        {
            puedeAtacar = false;

            // Cambio de opacidad para feedback visual
            Color c = spriteRenderer.color;
            c.a = 0.5f;
            spriteRenderer.color = c;

            // Lógica de daño
            GameManager.Instance.PerderVida();
            other.gameObject.GetComponent<PlayerController>().AplicarGolpe();

            // Reactivar ataque tras cooldown
            Invoke(nameof(ReactivarAtaque), cooldownAtaque);
        }
    }

    void ReactivarAtaque()
    {
        puedeAtacar = true;

        // Restaurar opacidad
        Color c = spriteRenderer.color;
        c.a = 1f;
        spriteRenderer.color = c;
    }
}

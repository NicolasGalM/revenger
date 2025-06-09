using UnityEngine;
using System.Collections; // Necesario para usar IEnumerator

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = pointB.position;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = (targetPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Verificar que ambos objetos estén activos y que el jugador no tenga ya un parent
            if (collision.transform != null && 
                collision.gameObject.activeInHierarchy && 
                gameObject.activeInHierarchy)
            {
                try
                {
                    collision.transform.SetParent(transform);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Error al conectar jugador a plataforma: {e.Message}");
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Verificar múltiples condiciones antes de proceder
            if (gameObject.activeInHierarchy && 
                collision.transform != null && 
                collision.gameObject.activeInHierarchy &&
                collision.transform.parent == transform) // Solo si realmente es hijo de esta plataforma
            {
                StartCoroutine(DetachAfterFrame(collision.transform));
            }
        }
    }

    IEnumerator DetachAfterFrame(Transform player)
    {
        // Verificar que tanto la plataforma como el jugador sigan existiendo
        if (player != null && gameObject.activeInHierarchy)
        {
            yield return null; // Espera un frame
            
            // Verificaciones exhaustivas después del frame
            if (player != null && 
                player.gameObject.activeInHierarchy && 
                gameObject.activeInHierarchy &&
                player.parent == transform) // Solo desconectar si aún es hijo de esta plataforma
            {
                try
                {
                    player.SetParent(null);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Error al desconectar jugador de plataforma: {e.Message}");
                }
            }
        }
    }
}
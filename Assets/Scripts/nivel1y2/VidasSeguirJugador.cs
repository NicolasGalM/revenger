using UnityEngine;

public class VidasSeguirJugador : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    private Vector3 offset; // Mantiene la posición fija en el eje Y
    public float velocidadSeguimiento = 2.0f; // 🔹 Ajusta la velocidad de seguimiento

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("❌ No se ha asignado el Player en VidasSeguirJugador.");
            return;
        }

        // Guardar la diferencia inicial entre el jugador y la UI de vidas
        offset = transform.position - player.position;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        // 🔹 Movimiento suavizado con Lerp (se acerca gradualmente al jugador)
        Vector3 nuevaPosicion = new Vector3(player.position.x + offset.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, nuevaPosicion, velocidadSeguimiento * Time.deltaTime);
    }
}

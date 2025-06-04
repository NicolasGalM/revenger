using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // El objeto a seguir (el jugador)
    public Vector3 offset = new Vector3(0, 2, -10); // Z en -10 para cámara 2D
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return; // Evita errores si el jugador no está asignado

        // Calcula la nueva posición
        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, offset.z);

        // Movimiento suavizado con Lerp
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}

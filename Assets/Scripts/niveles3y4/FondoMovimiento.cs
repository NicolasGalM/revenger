using UnityEngine;

public class FondoMovimiento : MonoBehaviour
{
    [SerializeField] private Vector2 velocidadMovimiento;
    private Vector2 offset;
    private Material material;
    private Transform camaraTransform;
    private Vector3 ultimaPosicionCamara;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        camaraTransform = Camera.main.transform;
        ultimaPosicionCamara = camaraTransform.position;
    }

    private void LateUpdate()
    {
        if (camaraTransform == null) return;

        // Mueve el fondo con la c�mara en el eje X
        transform.position = new Vector3(camaraTransform.position.x, transform.position.y, transform.position.z);

        // Calcula el desplazamiento de la c�mara para ajustar la textura
        Vector3 desplazamiento = camaraTransform.position - ultimaPosicionCamara;
        offset = new Vector2(desplazamiento.x * velocidadMovimiento.x, desplazamiento.y * velocidadMovimiento.y);

        // Aplica el desplazamiento de la textura
        material.mainTextureOffset += offset;

        // Actualiza la posici�n de la c�mara para el siguiente frame
        ultimaPosicionCamara = camaraTransform.position;
    }
}
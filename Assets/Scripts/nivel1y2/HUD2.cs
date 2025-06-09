using UnityEngine;
using TMPro;

public class HUD2 : MonoBehaviour
{
    public TextMeshProUGUI puntos;
    public GameObject[] vidas;

    public void ActualizarPuntos(int puntosTotales, int objetivo = -1)
    {
        if (objetivo > 0)
            puntos.text = $"{puntosTotales} / {objetivo}";
        else
            puntos.text = puntosTotales.ToString();
    }

    public void DesactivarVida(int indice)
    {
        if (indice >= 0 && indice < vidas.Length)
            vidas[indice].SetActive(false);
    }

    public void ActivarVida(int indice)
    {
        if (indice >= 0 && indice < vidas.Length)
            vidas[indice].SetActive(true);
    }

    public void ResetVidas(int cantidad)
    {
        for (int i = 0; i < vidas.Length; i++)
        {
            vidas[i].SetActive(i < cantidad);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SelectorDeNiveles : MonoBehaviour
{
    public Button botonNivel1;
    public Button botonNivel2;
    public Button botonNivel3;
    public Button botonNivel4;
    public Button botonSalir;

    void Start()
    {
        int nivelDesbloqueado = PlayerPrefs.GetInt("nivelDesbloqueado", 1);

        ConfigurarBoton(botonNivel1, 1, nivelDesbloqueado);
        ConfigurarBoton(botonNivel2, 2, nivelDesbloqueado);
        ConfigurarBoton(botonNivel3, 3, nivelDesbloqueado);
        ConfigurarBoton(botonNivel4, 4, nivelDesbloqueado);

        // Botón de salida
        botonSalir.onClick.AddListener(() => SceneManager.LoadScene("MenuInicial"));
    }

    void ConfigurarBoton(Button boton, int nivel, int nivelDesbloqueado)
    {
        bool desbloqueado = nivelDesbloqueado >= nivel;
        boton.interactable = desbloqueado;

        // Accedemos al texto del botón y lo ocultamos si está bloqueado
        TextMeshProUGUI texto = boton.GetComponentInChildren<TextMeshProUGUI>();
        if (texto != null)
        {
            texto.alpha = desbloqueado ? 1f : 0f; // 0 = invisible, 1 = visible
        }

        if (desbloqueado)
        {
            boton.onClick.AddListener(() => CargarNivel("Nivel" + nivel));
        }
    }

    void CargarNivel(string nombreEscena)
{
    Debug.Log("Cargando nivel: " + nombreEscena);
    SceneManager.LoadScene(nombreEscena);
}


}

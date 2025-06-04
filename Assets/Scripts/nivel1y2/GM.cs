using UnityEngine;
using UnityEngine.SceneManagement;


public class GM : MonoBehaviour
{
    public static GM instance;

    public int vidas = 3;

    private int puntosTotales;  // monedas esta vida
    private int monedasAcumuladasNivel;  // acumuladas todas las vidas

    public int PuntosTotales { get { return puntosTotales; } }
    public int MonedasAcumuladasNivel { get { return monedasAcumuladasNivel; } }

    [Header("Referencias UI")]
    public HUD2 hud;

    void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hud = FindObjectOfType<HUD2>();
        if (hud != null)
        {
            for (int i = 0; i < hud.vidas.Length; i++)
                if (i < vidas) hud.ActivarVida(i);
                else hud.DesactivarVida(i);

            hud.ActualizarPuntos(puntosTotales);
        }
    }

    public void ReduceVidas()
    {
        vidas--;
        Debug.Log($"⚠️ Vida reducida. Vidas restantes: {vidas}");

        if (hud != null && vidas >= 0 && vidas < hud.vidas.Length)
            hud.DesactivarVida(vidas);

        if (vidas <= 0)
        {
            Debug.Log("💀 Sin vidas. Reseteando todo y volviendo a niveles.");
            ResetTodo();  // reset monedas y vidas
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            // Reinicia solo la escena actual, reinicia puntos esta vida pero no el acumulado
            ResetPuntosVida();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ResetPuntosVida()
    {
        puntosTotales = 0;
        if (hud != null)
            hud.ActualizarPuntos(puntosTotales);
    }

    public void ResetTodo()
    {
        vidas = 3;
        puntosTotales = 0;
        monedasAcumuladasNivel = 0;

        if (hud != null)
        {
            for (int i = 0; i < hud.vidas.Length; i++)
                hud.ActivarVida(i);

            hud.ActualizarPuntos(puntosTotales);
        }
    }

    public void SumarPuntos(int puntosASumar)
    {
        puntosTotales += puntosASumar;
        monedasAcumuladasNivel += puntosASumar;

        Debug.Log($"Monedas esta vida: {puntosTotales}, acumuladas en nivel: {monedasAcumuladasNivel}");

        if (hud != null)
            hud.ActualizarPuntos(puntosTotales);
    }
}

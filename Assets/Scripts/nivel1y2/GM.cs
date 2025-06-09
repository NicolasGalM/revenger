using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour
{
    public static GM instance;

    public int vidas = 3;
    public int objetivoPuntos = 30;

    private int puntosTotales = 0;  // Solo puntos de la vida actual
    private int toquesActuales = 0; // Toques actuales con EnemyFire

    public int PuntosTotales { get { return puntosTotales; } }
    public int ToquesActuales { get { return toquesActuales; } }

    [Header("Referencias UI")]
    public HUD2 hud;

    [Header("Invencibilidad")]
    public bool invencible = false;
    public float tiempoInvencible = 5f;

    void Awake()
    {
        Debug.Log($"🧪 GM Awake - instancia actual: {this.gameObject.name}");

        if (instance != null && instance != this)
        {
            Debug.Log($"🧪 GM duplicado detectado, destruyendo {this.gameObject.name}");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
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
        ActualizarHUD();
    }

    void ActualizarHUD()
    {
        if (hud != null)
        {
            for (int i = 0; i < hud.vidas.Length; i++)
                if (i < vidas) hud.ActivarVida(i);
                else hud.DesactivarVida(i);

            hud.ActualizarPuntos(puntosTotales, objetivoPuntos);
            
            // Si el HUD tiene método para mostrar toques, actualizarlo
            // hud.ActualizarToques(toquesActuales); // Descomenta si implementas esto en HUD2
        }
    }

    public void RecibirDaño()
    {
        if (invencible)
        {
            Debug.Log("🛡️ Daño recibido pero estamos invencibles.");
            return;
        }

        ReduceVidas();

        if (vidas > 0)
        {
            StartCoroutine(InvencibleTemporario());
        }
    }

    private IEnumerator InvencibleTemporario()
    {
        invencible = true;
        Debug.Log("🛡️ Invencibilidad activada.");
        yield return new WaitForSeconds(tiempoInvencible);
        invencible = false;
        Debug.Log("🛡️ Invencibilidad desactivada.");
    }

    public void ReduceVidas()
    {
        vidas--;
        Debug.Log($"⚠️ Vida reducida. Vidas restantes: {vidas}");

        if (hud != null && vidas >= 0 && vidas < hud.vidas.Length)
            hud.DesactivarVida(vidas);

        if (vidas <= 0)
        {
            Debug.Log("💀 Sin vidas. Reseteando todo y yendo a GameOver.");
            ResetTodo();
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            // Comentado para no resetear puntos al perder vida:
            // ResetPuntosVida();

            // No recargamos la escena para evitar reinicio total.
            // Aquí podrías poner efectos visuales/audio de daño.
        }
    }

    public void ResetPuntosVida()
    {
        puntosTotales = 0;
        if (hud != null)
            hud.ActualizarPuntos(puntosTotales, objetivoPuntos);
    }

    public void ResetTodo()
    {
        vidas = 3;
        puntosTotales = 0;
        toquesActuales = 0; // Resetear toques también
        invencible = false;

        if (hud != null)
        {
            for (int i = 0; i < hud.vidas.Length; i++)
                hud.ActivarVida(i);
            hud.ActualizarPuntos(puntosTotales, objetivoPuntos);
            // hud.ActualizarToques(toquesActuales); // Descomenta si implementas esto en HUD2
        }
    }

    public void SumarPuntos(int puntosASumar)
    {
        puntosTotales += puntosASumar;
        Debug.Log($"🪙 Puntos actuales esta vida: {puntosTotales}");
        if (hud != null)
            hud.ActualizarPuntos(puntosTotales, objetivoPuntos);
    }

    // Método para actualizar los toques actuales
    public void ActualizarToques(int toques)
    {
        toquesActuales = toques;
        Debug.Log($"🔥 Toques EnemyFire actualizados: {toquesActuales}");
        
        // Si el HUD tiene método para mostrar toques, actualizarlo
        // if (hud != null)
        //     hud.ActualizarToques(toquesActuales);
    }

    // Método para llamar en caída al vacío
    public void CaidaAlVacio()
    {
        Debug.Log("⬇️ Caída al vacío detectada. Game Over.");
        ResetTodo();
        SceneManager.LoadScene("GameOver");
    }
    
    // Método público para obtener toques actuales (útil para otros scripts)
    public int GetToquesActuales()
    {
        return toquesActuales;
    }
}
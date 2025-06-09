using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Globalization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("HUD & Points")]
    public HUD hud;
    public int PuntosTotales { get; private set; }

    [Header("Lives")]
    [Tooltip("Número inicial de vidas")]
    public int vidas = 3;

    [Header("Fall Death Settings")]
    [Tooltip("Transform del jugador para chequear su posición Y")]
    public Transform playerTransform;
    [Tooltip("Coordenada Y mínima antes de morir al instante")]
    public float lethalY = -10f;

    [Header("Game Over UI")]
    [Tooltip("Panel de Game Over que se activa al morir sin vidas")]
    public GameObject gameOverUI;

    [Header("Nivel Info")]
    public int levelId = 0;
    public string escenaNiveles = "Niveles"; // Nombre de la escena del selector de niveles

    private Dictionary<string, int> monedasPorNivel = new Dictionary<string, int>
    {
        { "Nivel3", 100 },
        { "Nivel4", 100 }
    };

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogWarning("¡Cuidado! Más de un GameManager en escena.");
    }

    private void Start()
    {
        // Estado inicial del Game Over UI y tiempo
        if (gameOverUI != null)
            gameOverUI.SetActive(false);
        Time.timeScale = 1f;

        // Asignar levelId automáticamente si no está configurado
        if (levelId <= 0)
        {
            string escena = SceneManager.GetActiveScene().name;
            switch (escena)
            {
                case "Nivel3": levelId = 3; break;
                case "Nivel4": levelId = 4; break;
            }
        }
    }

    private void Update()
    {
        // Solo evaluar muerte por caída si aún hay vidas
        if (vidas > 0 && playerTransform != null && playerTransform.position.y < lethalY)
        {
            PerderVida();
        }
    }

   public void SumarPuntos(int puntosASumar)
{
    PuntosTotales += puntosASumar;

    string escenaActual = SceneManager.GetActiveScene().name;
    int puntosMeta = 0;

    if (monedasPorNivel.TryGetValue(escenaActual, out puntosMeta))
    {
        if (hud != null)
            hud.ActualizarPuntos(PuntosTotales, puntosMeta); // nuevo método con dos argumentos
    }
    else
    {
        if (hud != null)
            hud.ActualizarPuntos(PuntosTotales); // fallback por si no encuentra meta
    }
}


    public void PerderVida()
    {
        if (vidas <= 0) return;

        vidas--;
        if (hud != null)
            hud.DesactivarVida(vidas);

        if (vidas <= 0)
        {
            GameOver();
        }
    }

    public bool RecuperarVida()
    {
        if (vidas >= 3) return false;

        int indexToActivate = vidas;
        vidas++;
        if (hud != null)
            hud.ActivarVida(indexToActivate);
        return true;
    }

    private void GameOver()
{
    Time.timeScale = 1f; // Asegura que el tiempo esté normal para la escena nueva
    SceneManager.LoadScene("GameOver");
}

    // UI Buttons
    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void VolverAlMenuPrincipal(int sceneIndex = 0)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneIndex);
    }

    // ✅ Lógica para enviar la sesión y volver a selector de niveles
    public void CompletarNivel()
    {
        StartCoroutine(ValidarYEnviarSesion());
    }

    private IEnumerator ValidarYEnviarSesion()
    {
        int playerId = PlayerPrefs.GetInt("player_id", -1);
        if (playerId == -1 || levelId <= 0)
        {
            Debug.LogError("❌ player_id o levelId inválido.");
            yield break;
        }

        string escenaActual = SceneManager.GetActiveScene().name;
        if (!monedasPorNivel.ContainsKey(escenaActual))
        {
            Debug.LogError("❌ No se ha definido el número total de monedas para esta escena.");
            yield break;
        }

        int coins = PuntosTotales;
        int totalCoins = monedasPorNivel[escenaActual];

        if (coins < totalCoins)
        {
            Debug.LogWarning("⚠️ No se recogieron todas las monedas. No se guarda la sesión. " + coins);
            yield break;
        }

        float tiempo = Time.timeSinceLevelLoad;
        string json = $"{{\"playerId\":{playerId},\"levelId\":{levelId},\"coinsEarned\":{coins},\"completionTime\":{tiempo.ToString(CultureInfo.InvariantCulture)}}}";

        byte[] body = Encoding.UTF8.GetBytes(json);

        Debug.Log($"📤 JSON enviado: {json}");

        UnityWebRequest request = new UnityWebRequest("http://localhost:3000/game-session", "POST");
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.Log("🌐 Resultado: " + request.result);
        Debug.Log("📥 Respuesta: " + request.downloadHandler.text);

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Sesión guardada correctamente.");
            DesbloquearSiguienteNivel(levelId);
            SceneManager.LoadScene(escenaNiveles);
        }
        else
        {
            Debug.LogError("❌ Error al guardar sesión: " + request.error);
        }
    }

    private void DesbloquearSiguienteNivel(int nivelActual)
    {
        int nivelDesbloqueado = PlayerPrefs.GetInt("nivelDesbloqueado", 1);
        if (nivelActual >= nivelDesbloqueado)
        {
            PlayerPrefs.SetInt("nivelDesbloqueado", nivelActual + 1);
            PlayerPrefs.Save();
            Debug.Log($"✅ Nivel {nivelActual + 1} desbloqueado.");
        }
    }
}

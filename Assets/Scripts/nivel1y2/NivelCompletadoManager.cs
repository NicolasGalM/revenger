using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

public class NivelCompletadoManager : MonoBehaviour
{
    public string escenaNiveles = "Niveles"; // Nombre de la escena de selección de niveles

    public int levelId = 0; // ID del nivel en la base de datos (ej: 1, 2, 3...)

    private Dictionary<string, int> monedasPorNivel = new Dictionary<string, int>
    {
        { "Nivel1", 30 },
        { "Nivel2", 26 },
        { "Nivel3", 6 }
    };

    void Start()
    {
        if (levelId <= 0)
        {
            string nombreEscena = SceneManager.GetActiveScene().name;

            switch (nombreEscena)
            {
                case "Nivel1": levelId = 1; break;
                case "Nivel2": levelId = 2; break;
                case "Nivel3": levelId = 3; break;
                default:
                    Debug.LogWarning("⚠️ Escena no mapeada a levelId");
                    break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ValidarYEnviarSesion());
        }
    }

    IEnumerator ValidarYEnviarSesion()
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

    // 👉 Cambiar aquí para usar la variable acumulada:
    int coins = GM.instance.MonedasAcumuladasNivel;
    int totalCoins = monedasPorNivel[escenaActual];

    if (coins < totalCoins)
    {
        Debug.LogWarning($"⚠️ No se recogieron todas las monedas ({coins}/{totalCoins}). No se guarda la sesión.");
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


    void DesbloquearSiguienteNivel(int nivelActual)
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

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class SessionManager : MonoBehaviour
{
    public static SessionManager instance;
    public int sessionId;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CrearSesion(int levelId)
    {
        StartCoroutine(CrearSesionRoutine(levelId));
    }

    IEnumerator CrearSesionRoutine(int levelId)
    {
        int playerId = PlayerPrefs.GetInt("player_id");
        string token = PlayerPrefs.GetString("access_token");

        string json = $"{{\"playerId\":{playerId},\"levelId\":{levelId}}}";
        byte[] body = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest("http://localhost:3000/game-sessions", "POST");
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Sesión creada: " + request.downloadHandler.text);
            // Parsear el sessionId si es necesario
            sessionId = JsonUtility.FromJson<SessionResponse>(request.downloadHandler.text).id;
        }
        else
        {
            Debug.LogError("Error creando sesión: " + request.error);
        }
    }

    [System.Serializable]
    public class SessionResponse
    {
        public int id;
    }
}

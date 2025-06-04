using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField userField;
    public TMP_InputField passwordField;
    public Button loginButton;
    public Button registerButton;
    public Button exitButton;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterClicked);
        exitButton.onClick.AddListener(OnExitClicked);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnLoginClicked();
        }
    }

    void OnLoginClicked()
    {
        StartCoroutine(LoginRoutine());
    }

    void OnRegisterClicked()
    {
        SceneManager.LoadScene("Register");
    }

    void OnExitClicked()
    {
        Application.Quit();
    }

    IEnumerator LoginRoutine()
    {
        string user = userField.text;
        string password = passwordField.text;

        string jsonBody = $"{{\"username\":\"{user}\",\"password\":\"{password}\"}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest("http://localhost:3000/auth/login", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Login successful!");
            Debug.Log(request.downloadHandler.text);

            // Aquí puedes guardar el token para usarlo luego (en PlayerPrefs, por ejemplo)
            // string token = JsonUtility.FromJson<TokenResponse>(request.downloadHandler.text).access_token;
            var response = JsonUtility.FromJson<TokenResponse>(request.downloadHandler.text);
            PlayerPrefs.SetString("access_token", response.access_token);
            PlayerPrefs.SetInt("player_id", response.player_id);
            Debug.Log("🔐 Player ID guardado: " + response.player_id);

            // Y pasar a la siguiente escena del juego
            SceneManager.LoadScene("MenuInicial");
        }
        else
        {
            Debug.LogError("Login failed: " + request.error);
        }
    }

    // Para parsear el JSON del token, si lo necesitas
    [System.Serializable]
    public class TokenResponse
    {
        public string access_token;
        public int player_id;
    }
}

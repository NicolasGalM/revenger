using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField userField;
    public TMP_InputField passwordField;
    public TMP_InputField emailField;
    public Button enterButton;
    public Button exitButton;  // <-- Botón salir

    private void Start()
    {
        enterButton.onClick.AddListener(OnEnterClicked);
        exitButton.onClick.AddListener(OnExitClicked);  // <-- Listener para salir
    }

    void OnEnterClicked()
    {
        StartCoroutine(RegisterRoutine());
    }

    void OnExitClicked()
    {
        Application.Quit();
    }

    IEnumerator RegisterRoutine()
    {
        string user = userField.text;
        string password = passwordField.text;
        string email = emailField.text;

        string jsonBody = $"{{\"username\":\"{user}\",\"password\":\"{password}\",\"email\":\"{email}\"}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest("http://localhost:3000/auth/register", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Register successful!");
            SceneManager.LoadScene("Verification");
        }
        else
        {
            Debug.LogError("Register failed: " + request.error);
        }
    }
}

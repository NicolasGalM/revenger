using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class VerificationManager : MonoBehaviour
{
    public TMP_InputField emailField;
    public TMP_InputField codeField;
    public Button enterButton;
    public Button backButton;

    private void Start()
    {
        enterButton.onClick.AddListener(OnEnterClicked);
        backButton.onClick.AddListener(OnBackClicked);
    }

    void OnEnterClicked()
    {
        StartCoroutine(VerificationRoutine());
    }

    void OnBackClicked()
    {
        SceneManager.LoadScene("Register");
    }

    IEnumerator VerificationRoutine()
    {
        string email = emailField.text;
        string otpCode = codeField.text;

        string jsonBody = $"{{\"email\":\"{email}\",\"otpCode\":\"{otpCode}\"}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest("http://localhost:3000/auth/verify-register", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Verification successful!");
            SceneManager.LoadScene("Login");
        }
        else
        {
            Debug.LogError("Verification failed: " + request.downloadHandler.text);
        }
    }
}

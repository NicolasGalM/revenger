using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    [Header("Loading Settings")]
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private float loadingDuration = 7f;
    [SerializeField] private string nextSceneName = "Login";
    
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip loadingAudio;
    [SerializeField] private bool loopAudio = true;
    [SerializeField] [Range(0f, 1f)] private float audioVolume = 0.5f;
    
    private void Start()
    {
        // Configurar el slider
        if (loadingSlider != null)
        {
            loadingSlider.value = 0f;
            loadingSlider.maxValue = 1f;
        }
        
        // Configurar y reproducir audio si está disponible
        SetupAudio();
        
        // Iniciar la corrutina de carga
        StartCoroutine(LoadingCoroutine());
    }
    
    private void SetupAudio()
    {
        if (audioSource != null && loadingAudio != null)
        {
            audioSource.clip = loadingAudio;
            audioSource.volume = audioVolume;
            audioSource.loop = loopAudio;
            audioSource.Play();
        }
    }
    
    private IEnumerator LoadingCoroutine()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < loadingDuration)
        {
            elapsedTime += Time.deltaTime;
            
            // Calcular el progreso (de 0 a 1)
            float progress = elapsedTime / loadingDuration;
            
            // Actualizar el slider
            if (loadingSlider != null)
            {
                loadingSlider.value = progress;
            }
            
            yield return null;
        }
        
        // Asegurar que el slider esté completamente lleno
        if (loadingSlider != null)
        {
            loadingSlider.value = 1f;
        }
        
        // Detener el audio antes de cambiar de escena
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        // Cambiar a la escena de login
        SceneManager.LoadScene(nextSceneName);
    }
    
    // Método opcional para cambiar la duración de carga desde otros scripts
    public void SetLoadingDuration(float duration)
    {
        loadingDuration = duration;
    }
    
    // Método opcional para cambiar la escena de destino
    public void SetNextScene(string sceneName)
    {
        nextSceneName = sceneName;
    }
}
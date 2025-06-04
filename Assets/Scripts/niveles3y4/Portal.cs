using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    

    // Opci�n B: cargar por �ndice
    [SerializeField] private int nextSceneBuildIndex = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Comprobar que quien toca es el jugador
        if (other.CompareTag("Player"))
        {
            // Prioriza �ndice si es v�lido (> = 0), sino usa nombre
            if (nextSceneBuildIndex >= 0)
                SceneManager.LoadScene(nextSceneBuildIndex);
            else
                Debug.LogWarning("No se ha configurado nextSceneName ni nextSceneBuildIndex");
        }
    }
}

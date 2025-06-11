using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public float duracion = 4f;

     void Start()
    {
        Invoke("VolverAlMenu", duracion);
    }


    public void VolverAlMenu()
{
    Destroy(GM.instance?.gameObject); // 💥 Destruye el GM manualmente
    Destroy(gameObject);
    SceneManager.LoadScene("Niveles");
}

}

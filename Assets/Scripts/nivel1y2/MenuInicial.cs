using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    

    public void Jugar(){
        SceneManager.LoadScene("Niveles");

    }

    public void Salir(){
        Debug.Log("Salir....");
        Application.Quit();

    }

    public void Ranking(){
        SceneManager.LoadScene("OpRanking");
    }

    
}

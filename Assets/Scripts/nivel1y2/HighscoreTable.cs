using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class HighscoreTable : MonoBehaviour
{
    [System.Serializable]
    public class EntryDisplay
    {
        public Text postText;     // Posición (1 al 10)
        public Text levelText;    // Nivel (ej: "Nivel 1")
        public Text scoreText;    // Tiempo (ej: "14.35s")
        public Text nameText;     // Jugador (ej: "juan")
    }

    public List<EntryDisplay> entryDisplays; // Asignar desde el Inspector, size = 10

    private void Start()
    {
        List<RankingManager.RankingEntry> ranking = RankingManager.currentRanking;

        Debug.Log("Start ejecutado. currentRanking tiene: " + ranking.Count + " entradas.");

        for (int i = 0; i < entryDisplays.Count; i++)
        {
            var display = entryDisplays[i];

            if (i < ranking.Count)
            {
                var entry = ranking[i];

                display.postText.text = (i + 1).ToString();
                display.levelText.text = entry.levelname;
                display.scoreText.text = $"{entry.completiontime:F2}s";
                display.nameText.text = entry.username;
            }
            else
            {
                // Oculta los que sobran o pon como vacío
                display.postText.text = "";
                display.levelText.text = "";
                display.scoreText.text = "";
                display.nameText.text = "";
            }
        }
    }

     public void ButtonExit()
    {
        SceneManager.LoadScene("OpRanking");
    }
}

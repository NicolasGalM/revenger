using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class RankingManager : MonoBehaviour
{
    public string baseUrl = "http://localhost:3000/game-session/ranking";
    public string rankingSceneName = "Ranking";
    public string menuSceneName = "MenuInicial";

    public static int selectedLevelId = -1;
    public static List<RankingEntry> currentRanking = new List<RankingEntry>();

    [System.Serializable]
    public class RankingEntry
    {
        public string username;
        public string levelname;
        public float completiontime;
    }

    [System.Serializable]
    public class RankingList
    {
        public RankingEntry[] data;
    }

    // Botones de selección de nivel
    public void OnClickLevel1() => StartCoroutine(LoadRankingAndGo(1));
    public void OnClickLevel2() => StartCoroutine(LoadRankingAndGo(2));
    public void OnClickLevel3() => StartCoroutine(LoadRankingAndGo(3));
    public void OnClickLevel4() => StartCoroutine(LoadRankingAndGo(4));
    public void OnClickTopGeneral() => StartCoroutine(LoadRankingAndGo(-1));
    public void OnClickExit() => SceneManager.LoadScene(menuSceneName);

    private IEnumerator LoadRankingAndGo(int levelId)
    {
        selectedLevelId = levelId;

        string url = (levelId == -1)
            ? $"{baseUrl}/general"
            : $"{baseUrl}/level/{levelId}";

        Debug.Log("⏳ Cargando datos desde: " + url);
        yield return StartCoroutine(FetchRanking(url));

        if (currentRanking != null && currentRanking.Count > 0)
        {
            Debug.Log("✅ Ranking listo. Cambiando a escena Ranking.");
        }
        else
        {
            Debug.LogWarning("⚠️ Ranking vacío o nulo.");
        }

        SceneManager.LoadScene(rankingSceneName);
    }

    private IEnumerator FetchRanking(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("JSON recibido: " + json);

            currentRanking = new List<RankingEntry>(
                JsonUtility.FromJson<RankingWrapper>("{\"data\":" + json + "}").data
            );

            Debug.Log("🎯 Datos parseados: " + currentRanking.Count + " entradas.");
        }
        else
        {
            Debug.LogError("❌ Error al obtener ranking: " + request.error);
            currentRanking = new List<RankingEntry>();
        }
    }

    [System.Serializable]
    private class RankingWrapper
    {
        public RankingEntry[] data;
    }
}

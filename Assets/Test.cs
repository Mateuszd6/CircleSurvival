using UnityEngine;

public class Test : MonoBehaviour
{
    private GameSettings gameSettings;

    void Awake()
    {
        gameSettings = Singles.GetScriptable<GameSettings>();
    }

    void Start() 
    {
#if false
        Debug.Log("Game settings: black circle prob: " + gameSettings.blackCircleProbability + "\n");
        Highscores hs = HighscoreManager.Instance.Highscores;
        Debug.Log(JsonUtility.ToJson(hs));

        HighscoreManager.Instance.AddScoreToHighscores("Maty3", 27);

        hs = HighscoreManager.Instance.Highscores;
        Debug.Log(JsonUtility.ToJson(hs));
#endif
    }
}

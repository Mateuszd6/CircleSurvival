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
        Debug.Log("Game settings: black circle prob: " + gameSettings.blackCircleProbability + "\n");
    }

    void Update() 
    {
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoSingleton<MenuManager>
{
    private enum MenuState { start, highscores };
    private MenuState menuState = MenuState.start;

    public void StateStart()
    {
        menuState = MenuState.start;

        foreach (GameObject go in startObjects)
            go.SetActive(true);

        foreach (GameObject go in highscoresObjects)
            go.SetActive(false);
    }

    public void StateHighscores()
    {
        menuState = MenuState.highscores;

        foreach (GameObject go in startObjects)
            go.SetActive(false);

        foreach (GameObject go in highscoresObjects)
            go.SetActive(true);
    }

    public GameObject[] startObjects;
    public GameObject[] highscoresObjects;

    void Start()
    {
        StateStart();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("game");
    }
}

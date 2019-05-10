using UnityEngine.SceneManagement;

public class MenuManager : MonoSingleton<MenuManager>
{
    public void StartGame()
    {
        SceneManager.LoadScene("game");
    }
}

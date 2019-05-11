using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    public GameSettings gameSettings;
    public RectTransform gameCanvasTransform;
    public Text gameTimer;
    public Text countdown;

    public Text looseInfo;
    public Text finalScoreInfo;
    public Text highscoreInfo;
    public InputField highscoreNameField;
    public Button submitScoreButton;

    public GameObject saveCircle;
    public GameObject deadlyCircle;

    private List<Circle> activeCircles; 

    private enum GameState { init, running, ended };
    private GameState gameState = GameState.init;

    private int scoredResult = 0;
    private int currentLevelIdx = 0;
    private float timeInLevel = 0;
    private float timeWithoutSpawn = 0;
    private float nextSpawnTime = 0.5f;
    private float currentTime = 0f;

    private string TimeInSecondsToString(int seconds)
    {
        int min = seconds / 60;
        int sec = seconds % 60;
        string minutesStr = min == 0 ? "" : min.ToString() + " min ";
        string secondsStr = sec.ToString() + " sec ";

        return minutesStr + secondsStr;
    }

    private Vector2 RandomScreenPosition(float circleSize)
    {
        var screenSize = new Vector2(gameCanvasTransform.rect.width,
                                     gameCanvasTransform.rect.height);

        // This offset is added to the position, so that the circle 
        // will never be right on the boundary.
        float offset = 5;
        float circleRadius = circleSize / 2;
        var positionBound = new Vector2(screenSize.x / 2 - circleRadius - offset, 
                                        screenSize.y / 2 - circleRadius - offset);

        float randomX = Random.Range(-positionBound.x, positionBound.x);
        float randomY = Random.Range(-positionBound.y, positionBound.y); 
        var randomPos = new Vector2(randomX, randomY);
        return randomPos;
    }

    private bool CircleIntersects(float checkedSize, Vector2 checkedPosition)
    {
        float checkedRadius = checkedSize / 2;
        foreach (Circle c in activeCircles)
        {
            float cRadius = c.Size / 2;

            // Diff is calculated so that we can call sqrMagitude and avoid 
            // calculating the square root.
            Vector2 diff = checkedPosition - c.Position;
            if (Vector2.SqrMagnitude(diff) <= (cRadius + checkedSize) * (cRadius + checkedSize))
            {
                Debug.LogWarning("Circle intersects!");
                return true;
            }
        }

        return false;
    }

    public void SpawnCircle()
    {
        float screenW = gameCanvasTransform.rect.width;
        float screenH = gameCanvasTransform.rect.height;

        // The id repetition is very unlikely but lets be pedantic about this.
        int newID = 0;
        do
        {
            newID = Random.Range(0, 2147483647);
        } while (activeCircles.FindIndex(x => x.Id == newID) != -1);

        float newSize = 0;
        float lifetime = Random.Range(gameSettings.levels[currentLevelIdx].dotLifetimeLenMin,
                                      gameSettings.levels[currentLevelIdx].dotLifetimeLenMax);
        Vector2 newPosition = Vector2.zero;

        // The max limit of trys is 32 which is veery high. But when with some reason it 
        // would be even harder to find a clear spot for the dot we just won't spawn it 
        // avoiding infinite loop.
        int trysLimit = 32;
        do
        {
            trysLimit--;
            newSize = Random.Range(screenH * 0.1f, screenH * 0.25f);
            newPosition = RandomScreenPosition(newSize);
        } while (trysLimit > 0 && CircleIntersects(newSize, newPosition));

        if (trysLimit <= 0)
            return;

        GameObject spawnedCirclePrefab =
            (Random.Range(0f, 1f) <= gameSettings.blackCircleProbability
             ? deadlyCircle 
             : saveCircle);

        var createdCircle = Instantiate(spawnedCirclePrefab, gameCanvasTransform.transform)
                                .GetComponent<Circle>();


        createdCircle.SetValues(newID, newSize, lifetime, newPosition);
        activeCircles.Add(createdCircle);
        createdCircle.ChangeState(Circle.CircleState.initializing);
        AudioManager.Instance.PlayPopSound();
    }

    public void DestroyCircle(int circleID)
    {
        // We don't have a way to remove and return reference in one lookup 
        // with LINQ, so we have to do it manually.
        int index = activeCircles.FindIndex(x => x.Id == circleID);
        Circle destroyedCircle = activeCircles[index];
        activeCircles.RemoveAt(index);

        destroyedCircle.ChangeState(Circle.CircleState.deinitializing);
    }

    // One circle explodes to the whole screen, the rest becomes inactive.
    // The game finishes with this call.
    public void ExplodeCircle(int circleID)
    {
        Circle explodeCircle = activeCircles.Find(x => x.Id == circleID);
        foreach (Circle c in activeCircles)
        {
            if (c != explodeCircle)
                c.ChangeState(Circle.CircleState.none);
        }

        explodeCircle.transform.SetSiblingIndex(int.MaxValue);
        explodeCircle.ChangeState(Circle.CircleState.grow);
        AudioManager.Instance.PlayCutdown();
        gameState = GameState.ended; // No more circles will be spawned.
    }

    // Cleanup after the explosion. This will destroy all game objects and carry the 
    // game state to the summary screen.
    public void Cleanup()
    {
        foreach (Circle c in activeCircles)
            Destroy(c.gameObject);
        activeCircles.Clear();

        bool wasHighscore = HighscoreManager.Instance.CheckIfHighscore(scoredResult);

        looseInfo.gameObject.SetActive(true);
        finalScoreInfo.gameObject.SetActive(true);
        submitScoreButton.gameObject.SetActive(true);

        finalScoreInfo.text = "Time survived: " + TimeInSecondsToString(scoredResult);

        if (wasHighscore)
        {
            highscoreInfo.gameObject.SetActive(true);
            highscoreNameField.gameObject.SetActive(true);
        }
        else
        {
            submitScoreButton.GetComponentInChildren<Text>().text = "Back to menu";
        }

        Debug.Log("Final result: " + scoredResult + (wasHighscore ? "(HS)" : "") + "\n");
    }

    public void ExitGameScreen()
    {
        Debug.Log("Exitting game screen\n");
        
        if (HighscoreManager.Instance.CheckIfHighscore(scoredResult))
        {
            string playerName = highscoreNameField.text;
            if (playerName == "")
                playerName = "Anon";

            HighscoreManager.Instance.AddScoreToHighscores(playerName, scoredResult);
        }

        SceneManager.LoadScene("start");
    }

    private void UpdateTime(int secondsSinceStart)
    {
        string timeStr = TimeInSecondsToString(secondsSinceStart);

        scoredResult = secondsSinceStart;
        gameTimer.text = "Time: " + timeStr;
    }

    private void SetCountDown(int value)
    {
        countdown.gameObject.SetActive(true);
        AudioManager.Instance.PlayCountdown(value - 1);
        if (1 <= value && value <= 3)
            countdown.text = value.ToString();
        else
            countdown.text = "GO!";
    }

    void Awake()
    {
        activeCircles = new List<Circle>();
    }

    void Start()
    {
        looseInfo.gameObject.SetActive(false);
        finalScoreInfo.gameObject.SetActive(false);
        highscoreInfo.gameObject.SetActive(false);
        highscoreNameField.gameObject.SetActive(false);
        submitScoreButton.gameObject.SetActive(false);
    }

    int countdownState = 0;
    void Update()
    {
        currentTime += Time.deltaTime; 
        if (gameState == GameState.init)
        {
            if (countdownState == 4 && currentTime >= 3 * 0.9f + 2f)
            {
                countdown.gameObject.SetActive(false);

                Debug.Log("Game started\n");
                currentTime = 0;
                gameState = GameState.running;
            }
            else if (countdownState == 3 && currentTime >= 3 * 0.9f)
            {
                countdownState = 4;
                SetCountDown(countdownState);
            }
            else if (countdownState == 2 && currentTime >= 2 * 0.9f)
            {
                countdownState = 3;
                SetCountDown(countdownState);
            }
            else if (countdownState == 1 && currentTime >=  0.9f)
            {
                countdownState = 2;
                SetCountDown(countdownState);
            }
            else if (countdownState == 0)
            {
                countdownState = 1;
                SetCountDown(countdownState);
            }
        }

        if (gameState == GameState.running)
        {
            timeWithoutSpawn += Time.deltaTime;
            timeInLevel += Time.deltaTime;

            // This check will allow us to update this text much lesser.
            int secondsSinceStart = Mathf.RoundToInt(currentTime);
            if (secondsSinceStart > scoredResult)
            {
                UpdateTime(secondsSinceStart);
            }

            if (timeWithoutSpawn >= nextSpawnTime) 
            {
                SpawnCircle();
                timeWithoutSpawn = 0f;

                nextSpawnTime = Random.Range(gameSettings.levels[currentLevelIdx].minNoSpawnInterval,
                                             gameSettings.levels[currentLevelIdx].maxNoSpawnInterval);
            }

            if (timeInLevel >= gameSettings.levels[currentLevelIdx].levelDuration)
            {
                Debug.Log("Time " + timeInLevel + ": level was finished!\n");
                timeInLevel = 0;
                currentLevelIdx++;
            }
        }
    }
}

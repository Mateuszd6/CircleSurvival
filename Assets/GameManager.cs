using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: Add lock object and make sure only one circle is destoryed at the time.
public class GameManager : MonoSingleton<GameManager>
{
    public GameSettings gameSettings;
    public RectTransform gameCanvasTransform; // TODO: Get it by find?
    public Text gameTimer;
    public Text countdown;

    public List<Circle> activeCircles; // TODO: private

    public enum GameState { init, running, ended };
    public GameState gameState = GameState.init;

    public GameObject saveCircle;
    public GameObject deadlyCircle;

    private int scoredResult = 0;
    private int currentLevelIdx = 0;
    private float timeInLevel = 0;
    private float timeWithoutSpawn = 0;
    private float nextSpawnTime = 0.5f;
    private float gameTime = 0f;

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
            float cRadius = c.size / 2;

            // Diff is calculated so that we can call sqrMagitude and avoid 
            // calculating the square root.
            Vector2 diff = checkedPosition - c.position;
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

        // TODO: Make sure the ids does not repead.
        int newID = Random.Range(0, 2147483647);
        float newSize = 0;
        Vector2 newPosition = Vector2.zero;

        // TODO: Add limits of trys.
        do
        {
            newSize = Random.Range(screenH * 0.1f, screenH * 0.25f);
            newPosition = RandomScreenPosition(newSize);
        } while (CircleIntersects(newSize, newPosition));

        GameObject spawnedCirclePrefab =
            (Random.Range(0, 100) < gameSettings.blackCircleProbability
             ? deadlyCircle 
             : saveCircle);

        var createdCircle = Instantiate(spawnedCirclePrefab, gameCanvasTransform.transform)
                                .GetComponent<Circle>();

        createdCircle.SetValues(newID, newSize, newPosition);
        activeCircles.Add(createdCircle);
        createdCircle.ChangeState(Circle.CircleState.initializing);
        AudioManager.Instance.PlayPopSound();
    }

    public void DestroyCircle(int circleID)
    {
        // We don't have a way to remove and return reference in one lookup 
        // with LINQ, so we have to do it manually.
        int index = activeCircles.FindIndex(x => x.id == circleID);
        Circle destroyedCircle = activeCircles[index];
        activeCircles.RemoveAt(index);

        destroyedCircle.ChangeState(Circle.CircleState.deinitializing);
    }

    // One circle explodes to the whole screen, the rest becomes inactive.
    // The game finishes with this call.
    public void ExplodeCircle(int circleID)
    {
        Circle explodeCircle = activeCircles.Find(x => x.id == circleID);
        foreach (Circle c in activeCircles)
        {
            if (c != explodeCircle)
                c.circleState = Circle.CircleState.none;
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

        Debug.Log("Final result: " + scoredResult + "\n");
    }

    private void UpdateTime(int secondsSinceStart)
    {
        int minutes = secondsSinceStart / 60;
        int seconds = secondsSinceStart % 60;
        string minutesStr = minutes == 0 ? "" : minutes.ToString() + " min ";
        string secondsStr = seconds.ToString() + " sec ";

        scoredResult = secondsSinceStart;
        gameTimer.text = "Time: " + minutesStr + secondsStr;
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
        // countdown.gameObject.SetActive(false);
    }

    int countdownState = 0;
    void Update()
    {
        gameTime += Time.deltaTime; // TODO: Rename gameTime to stateTime etc
        if (gameState == GameState.init)
        {
            if (countdownState == 4 && gameTime >= 3 * 0.9f + 2f)
            {
                countdown.gameObject.SetActive(false);

                Debug.Log("Game started\n");
                gameTime = 0;
                gameState = GameState.running;
            }
            else if (countdownState == 3 && gameTime >= 3 * 0.9f)
            {
                countdownState = 4;
                SetCountDown(countdownState);
            }
            else if (countdownState == 2 && gameTime >= 2 * 0.9f)
            {
                countdownState = 3;
                SetCountDown(countdownState);
            }
            else if (countdownState == 1 && gameTime >=  0.9f)
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
            int secondsSinceStart = Mathf.RoundToInt(gameTime);
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

            if (timeInLevel >= gameSettings.levels[currentLevelIdx].endofTime)
            {
                Debug.Log("Time " + timeInLevel + ": level was finished!\n");
                timeInLevel = 0;
                currentLevelIdx++;
            }
        }
    }
}

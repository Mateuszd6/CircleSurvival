using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public RectTransform gameCanvasTransform; // TODO: Get it by find?
    private List<Circle> activeCircles;

    public GameObject saveCircle;
    public GameObject deadlyCircle;

    void Awake()
    {
        activeCircles = new List<Circle>();
    }

    Vector2 RandomScreenPosition(float circleSize)
    {
        var screenSize = new Vector2(gameCanvasTransform.rect.width,
                                     gameCanvasTransform.rect.height);

        // This offset is added to the position, so that the circle 
        // will never be right on the boundary.
        float offset = 5;
        float circleRadius = circleSize / 2;
        var randomPos = new Vector2(Random.Range(circleRadius + offset,
                                                 screenSize.x - circleRadius - offset),
                                    Random.Range(circleRadius + offset,
                                                 screenSize.y - circleRadius - offset));

        return randomPos;
    }

    public void SpawnCircle()
    {
        float screenW = gameCanvasTransform.rect.width;
        float screenH = gameCanvasTransform.rect.height;

        // TODO: Make sure the ids does not repead.
        int newID = Random.Range(0, 2147483647);
        float newSize = Random.Range(screenH * 0.1f, screenH * 0.25f);
        Vector2 newPosition = RandomScreenPosition(newSize);

        var createdCircle = Instantiate(saveCircle, gameCanvasTransform.transform)
                                .GetComponent<Circle>();

        createdCircle.SetValues(newID, newSize, newPosition);
        createdCircle.currentTime = 0.0f; // TODO: Remove it from here once change state fucntion gots implements.
        createdCircle.circleState = Circle.CircleState.initializing;
    }

    public void DestroyCircle(int circleID)
    {
        activeCircles.RemoveAll(x => x.id == circleID);
    }

    int alreadySpawned = 0;
    void Update()
    {
        if ((alreadySpawned <= 0 && Time.time >= 3.0f)
            || (alreadySpawned <= 1 && Time.time >= 3.5f)
            || (alreadySpawned <= 2 && Time.time >= 4.0f)
            || (alreadySpawned <= 3 && Time.time >= 4.5f)
            || (alreadySpawned <= 4 && Time.time >= 5.0f)
            || (alreadySpawned <= 5 && Time.time >= 5.5f)
            || (alreadySpawned <= 6 && Time.time >= 6.0f)
            || (alreadySpawned <= 7 && Time.time >= 7.0f)
            || (alreadySpawned <= 8 && Time.time >= 8.0f))
        {
            SpawnCircle();
            alreadySpawned = 15;
        }
    }
}

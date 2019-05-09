using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Circle : MonoBehaviour, IPointerClickHandler
{
    public Image greenFill;
    public RectTransform gameCanvasTransform;

    public int id;
    public float size = 0.0f;
    public float currentTime = 0.0f;
    public float initTime = 0.2f;
    public float expireTime = 4.0f;

    // When (de)initializing circle size lerps from(to) zero but the object is inactive. 
    // This make is 'smoothely' appear on the screen.
    public enum CircleState { initializing, couting, deinitializing };
    public CircleState circleState = CircleState.initializing;
    
    Vector2 RandomScreenPosition(float circleSize)
    {
        var screenSize = new Vector2(gameCanvasTransform.rect.width, 
                                     gameCanvasTransform.rect.height);

        // This offset is added to the position, so that the circle 
        // will never be right on the boundary.
        float offset = 5;
        float circleRadius = circleSize / 2;
        var randomPos = new Vector2(Random.Range(circleRadius + offset, screenSize.x - circleRadius - offset), 
                                    Random.Range(circleRadius + offset, screenSize.y - circleRadius - offset));

        return randomPos;
    }

    void Start()
    {
        id = Random.Range(0, 2147483647);
        var selfRectTransfrom = GetComponent<RectTransform>();

        float w = gameCanvasTransform.rect.width;
        float h = gameCanvasTransform.rect.height;
        float circlew = selfRectTransfrom.rect.width;
        float circleh = selfRectTransfrom.rect.height;


        var gameSettings = Singles.GetScriptable<GameSettings>();
        size = Random.Range(gameSettings.circleSizeMin, gameSettings.circleSizeMax);
        selfRectTransfrom.sizeDelta = new Vector2(size, size);
        Vector2 randomPosition = RandomScreenPosition(size);
        selfRectTransfrom.position = new Vector3(randomPosition.x, randomPosition.y);
        Debug.Log("Size: (" + w + "; " + h + "), Circle: (" + circlew + "; " + circleh + ")" + "\n");
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (circleState == CircleState.initializing)
        {
            var selfRectTransfrom = GetComponent<RectTransform>();
            float newScale = Mathf.Lerp(0.0f, size, currentTime / 0.08f);
            selfRectTransfrom.sizeDelta = new Vector2(newScale, newScale);

            if (newScale >= size)
            {
                currentTime = 0;
                circleState = CircleState.couting;
            }
        }
        
        if (circleState == CircleState.couting)
        {
            greenFill.fillAmount = Mathf.Lerp(1, 0, currentTime / expireTime);

            if (greenFill.fillAmount <= Mathf.Epsilon)
            {
                Debug.Log("Not clicked! Game should be finished!\n");
                Debug.Break();

                // TODO: We won't need these.
                currentTime = 0;
                circleState = CircleState.deinitializing;
            }
        }

        if (circleState == CircleState.deinitializing)
        {
            var selfRectTransfrom = GetComponent<RectTransform>();
            float newScale = Mathf.Lerp(size, 0.0f, currentTime / 0.08f);
            selfRectTransfrom.sizeDelta = new Vector2(newScale, newScale);

            if (newScale <= Mathf.Epsilon)
                Destroy(gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Circle " + id + " was clicked!\n");
        circleState = CircleState.deinitializing;
        currentTime = 0;
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// TODO: Stop calling get component everywere!
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
    // This make is 'smoothely' appear on the screen. Grow is done when the 'bomb' explodeds
    // covering all screen the the circle.
    public enum CircleState { initializing, couting, deinitializing, grow };
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
        size = Random.Range(h * 0.1f, h * 0.25f);
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
                circleState = CircleState.grow;
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

        if (circleState == CircleState.grow)
        {
            // This will get the large enough size to cover the whole screen.
            float screenCoverSize = 4 * Mathf.Max(gameCanvasTransform.rect.width, 
                                                  gameCanvasTransform.rect.height);

            var selfRectTransfrom = GetComponent<RectTransform>();
            float newScale = Mathf.Lerp(size, screenCoverSize, currentTime / 0.2f);
            selfRectTransfrom.sizeDelta = new Vector2(newScale, newScale);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // We allow the user to click on the initialized circle but this is unlikely 
        // (he has few frames for that and the object is small).
        if (circleState == CircleState.initializing
            || circleState == CircleState.couting)
        {
            Debug.Log("Circle " + id + " was clicked!\n");
            circleState = CircleState.deinitializing;
            currentTime = 0;
        }
    }
}

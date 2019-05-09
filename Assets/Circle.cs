using UnityEngine;
using UnityEngine.UI;

public class Circle : MonoBehaviour
{
    public Image greenFill;
    public RectTransform gameCanvasTransform;

    public float currentTime = 0.0f;
    public float expireTime = 4.0f;
    
    Vector2 RandomScreenPosition(float circleSize)
    {
        var screenSize = new Vector2(gameCanvasTransform.rect.width, 
                                     gameCanvasTransform.rect.height);

        var randomPos = new Vector2(Random.Range(circleSize, screenSize.x), 
                                    Random.Range(circleSize, screenSize.y));

        return randomPos;
    }

    void Start()
    {
        var selfRectTransfrom = GetComponent<RectTransform>();

        float w = gameCanvasTransform.rect.width;
        float h = gameCanvasTransform.rect.height;
        float circlew = selfRectTransfrom.rect.width;
        float circleh = selfRectTransfrom.rect.height;
        selfRectTransfrom.sizeDelta = new Vector2(120, 120);

        Vector2 randomPosition = RandomScreenPosition(120);
        selfRectTransfrom.position = new Vector3(randomPosition.x, randomPosition.y);
        Debug.Log("Size: (" + w + "; " + h + "), Circle: (" + circlew + "; " + circleh + ")" + "\n");
    }

    void Update()
    {
        if (greenFill.fillAmount > 0)
        {
            currentTime += Time.deltaTime;
            greenFill.fillAmount = Mathf.Lerp(1, 0, currentTime / expireTime);
        }

        if (currentTime > 4)
        {
            currentTime = 0;
            greenFill.fillAmount = 1;
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// TODO: Stop calling get component everywere!
public abstract class Circle : MonoBehaviour, IPointerClickHandler
{
    public RectTransform selfTransform;
    public CircleSettings settings;

    // These must be set by the external creator. 
    public int id;
    public float size;
    public float lifeTime;
    public Vector2 position;

    public float currentTime = 0f;

    // When (de)initializing circle size lerps from(to) zero but the object is inactive. 
    // This make is 'smoothely' appear on the screen. Grow is done when the 'bomb' explodeds
    // covering all screen the the circle. 
    public enum CircleState { none, initializing, couting, deinitializing, grow };
    public CircleState circleState = CircleState.none;

    public void ChangeState(CircleState newState)
    {
        if (circleState == newState)
        {
            Debug.LogWarning("The circle state is already " + newState.ToString() + "\n");
            return;
        }

        currentTime = 0;
        circleState = newState;
    }

    public abstract void HandleAction();

    public abstract void CountingUpdate();

    public virtual void InitUpdate()
    {
        var selfRectTransfrom = selfTransform;
        float newScale = Mathf.Lerp(0.0f, size, currentTime / settings.initTime);
        selfRectTransfrom.sizeDelta = new Vector2(newScale, newScale);

        if (currentTime >= settings.initTime)
        {
            ChangeState(CircleState.couting);
        }
    }

    public virtual void DeinitUpdate()
    {
        var selfRectTransfrom = selfTransform;
        float newScale = Mathf.Lerp(size, 0.0f, currentTime / settings.deinitTime);
        selfRectTransfrom.sizeDelta = new Vector2(newScale, newScale);

        if (currentTime >= settings.deinitTime)
            Destroy(gameObject);
    }

    // TODO: Make separate state for shaking. 
    private bool shake = true; // TODO: get rid of.
    public virtual void GrowUpdate()
    {
        // TODO: Find a way to skip get component here!
        var gameCanvasTransform = transform.parent.GetComponent<RectTransform>();

        if (shake)
        {
            // This fraction of the screen is empirical but looks good with many resolutions.
            var perc = gameCanvasTransform.rect.width / 140;
            Vector2 shakeOffset = new Vector2(Random.value * perc, Random.value * perc);
            selfTransform.localPosition = new Vector3(position.x + shakeOffset.x, position.y + shakeOffset.y);

            if (currentTime >= settings.shakeTime)
            {
                currentTime = 0;
                shake = false;
                AudioManager.Instance.PlayExplosion();
            }

            return;
        }

        float screenCoverSize = 4 * Mathf.Max(gameCanvasTransform.rect.width,
                                              gameCanvasTransform.rect.height);

        float newScale = Mathf.Lerp(size, screenCoverSize, currentTime / settings.growTime);
        selfTransform.sizeDelta = new Vector2(newScale, newScale);

        if (currentTime >= settings.growTime)
        {
            // TODO: Change this behaviour!
            Debug.Log("Growing has finished\n");
            ChangeState(CircleState.none);

            // The cleanup will delete all circle (including us).
            GameManager.Instance.Cleanup();
        }
    }
    
    public void SetValues(int newID, float newSize, float newLifetime, Vector2 newPosition)
    {
        id = newID;
        size = newSize;
        position = newPosition;
        lifeTime = newLifetime;

        Debug.Log("Local position: " + position + "\n");
        selfTransform.localPosition = new Vector3(position.x, position.y);
    }

    void Awake()
    {
        selfTransform = GetComponent<RectTransform>();

        // When the circle is instantiated we must set the scale to 0.
        // It will be reset by the creator.
        selfTransform.sizeDelta = new Vector2(0, 0);
    }

    // TODO: Rewrite them as coroutines?
    void Update()
    {
        currentTime += Time.deltaTime;

        if (circleState == CircleState.initializing)
        {
            InitUpdate();
        }
        
        if (circleState == CircleState.couting)
        {
            CountingUpdate();
        }

        if (circleState == CircleState.deinitializing)
        {
            DeinitUpdate();
        }

        if (circleState == CircleState.grow)
        {
            GrowUpdate();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // We allow the user to click on the initialized circle but this is unlikely 
        // (he has few frames for that and the object is small).
        if (circleState == CircleState.initializing
            || circleState == CircleState.couting)
        {
            HandleAction();
        }
    }
}

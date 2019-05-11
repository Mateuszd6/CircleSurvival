using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Circle : MonoBehaviour, IPointerClickHandler
{
    public CircleSettings settings;

    // When (de)initializing circle size lerps from(to) zero but the object is inactive. 
    // This make is 'smoothely' appear on the screen. Grow is done when the 'bomb' explodeds
    // covering all screen the the circle. 
    public enum CircleState { none, initializing, couting, deinitializing, grow };
    protected CircleState circleState = CircleState.none;

    protected RectTransform selfTransform;
    protected RectTransform parentTransform; // This is the game area in which circles spawn.

    protected float currentTime = 0f;

    // These must be set by the external creator. 
    protected int id;
    protected float size;
    protected float lifeTime;
    protected Vector2 position;

    public int Id { get { return id; } }
    public float Size { get { return size; } }
    public float LifeTime { get { return lifeTime; } }
    public Vector2 Position { get { return position; } }

    // Used when handling explode routine.
    private bool isShaking = true;

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

    public virtual void GrowUpdate()
    {
        if (isShaking)
        {
            // This fraction of the screen is empirical but looks good with many resolutions.
            var perc = parentTransform.rect.width / 140;
            Vector2 shakeOffset = new Vector2(Random.value * perc, Random.value * perc);
            selfTransform.localPosition = new Vector3(position.x + shakeOffset.x, 
                                                      position.y + shakeOffset.y);

            if (currentTime >= settings.shakeTime)
            {
                currentTime = 0;
                isShaking = false;
                AudioManager.Instance.PlayExplosion();
            }

            return;
        }

        float screenCoverSize = 4 * Mathf.Max(parentTransform.rect.width,
                                              parentTransform.rect.height);

        float newScale = Mathf.Lerp(size, screenCoverSize, currentTime / settings.growTime);
        selfTransform.sizeDelta = new Vector2(newScale, newScale);

        if (currentTime >= settings.growTime)
        {
            // We are done. Now cleanup will delete all circle (including us).
            ChangeState(CircleState.none);
            GameManager.Instance.Cleanup();
        }
    }
    
    public void SetValues(int newID, float newSize, float newLifetime, Vector2 newPosition)
    {
        id = newID;
        size = newSize;
        position = newPosition;
        lifeTime = newLifetime;

        selfTransform.localPosition = new Vector3(position.x, position.y);
    }

    void Awake()
    {
        selfTransform = GetComponent<RectTransform>();
        parentTransform = transform.parent.GetComponent<RectTransform>();

        // When the circle is instantiated we must set the scale to 0.
        // It will be reset by the creator.
        selfTransform.sizeDelta = new Vector2(0, 0);
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        switch(circleState)
        {
            case CircleState.initializing:
                InitUpdate();
                break;

            case CircleState.couting:
                CountingUpdate();
                break;

            case CircleState.deinitializing:
                DeinitUpdate();
                break;

            case CircleState.grow:
                GrowUpdate();
                break;

            default:
                break;
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

﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// TODO: Stop calling get component everywere!
public abstract class Circle : MonoBehaviour, IPointerClickHandler
{
    public RectTransform selfTransform;

    // These must be set by the external creator. 
    public int id;
    public float size;
    Vector2 position;

    public float currentTime = 0.0f;
    public float initTime = 0.08f; // TODO: Use this instead of magic values!
    public float deinitTime = 0.08f;
    public float growTime = 0.2f; 
    public float expireTime = 4.0f;

    // When (de)initializing circle size lerps from(to) zero but the object is inactive. 
    // This make is 'smoothely' appear on the screen. Grow is done when the 'bomb' explodeds
    // covering all screen the the circle. 
    public enum CircleState { none, initializing, couting, deinitializing, grow };
    public CircleState circleState = CircleState.none;

    public abstract void HandleAction();

    public abstract void CountingUpdate();

    public virtual void InitUpdate()
    {
        var selfRectTransfrom = selfTransform;
        float newScale = Mathf.Lerp(0.0f, size, currentTime / initTime);
        selfRectTransfrom.sizeDelta = new Vector2(newScale, newScale);

        if (currentTime >= initTime)
        {
            currentTime = 0;
            circleState = CircleState.couting;
        }
    }

    public virtual void DeinitUpdate()
    {
        var selfRectTransfrom = selfTransform;
        float newScale = Mathf.Lerp(size, 0.0f, currentTime / deinitTime);
        selfRectTransfrom.sizeDelta = new Vector2(newScale, newScale);

        if (currentTime >= deinitTime)
            Destroy(gameObject);
    }

    public virtual void GrowUpdate()
    {
        // This will get the large enough size to cover the whole screen.
        float screenCoverSize = 10000;
#if false
            4 * Mathf.Max(gameCanvasTransform.rect.width,
                                              gameCanvasTransform.rect.height);
#endif

        float newScale = Mathf.Lerp(size, screenCoverSize, currentTime / growTime);
        selfTransform.sizeDelta = new Vector2(newScale, newScale);

        if (currentTime >= growTime)
        {
            // TODO: Change this behaviour!
            Debug.Log("Growing has finished\n");
            Destroy(this);
        }
    }
    
    public void SetValues(int newID, float newSize, Vector2 newPosition)
    {
        id = newID;
        size = newSize;
        position = newPosition;
    }

    void Awake()
    {
        selfTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        // selfTransform.sizeDelta = new Vector2(size, size);
        // selfTransform.position = new Vector3(position.x, position.y);
    }

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

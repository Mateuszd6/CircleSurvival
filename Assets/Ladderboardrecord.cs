using UnityEngine;

public class Ladderboardrecord : MonoBehaviour
{
    void Start()
    {
        RectTransform selfTransform = GetComponent<RectTransform>();
        RectTransform parentTransform = transform.parent.parent.GetComponent<RectTransform>();
        selfTransform.sizeDelta = new Vector2(selfTransform.rect.width, parentTransform.rect.height / 5);
    }
}

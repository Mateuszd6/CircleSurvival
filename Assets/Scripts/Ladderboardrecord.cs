using UnityEngine;
using UnityEngine.UI;

public class Ladderboardrecord : MonoBehaviour
{
    private Text content;

    void Awake()
    {
        content = GetComponent<Text>();
    }

    void Start()
    {
        RectTransform selfTransform = GetComponent<RectTransform>();
        RectTransform parentTransform = transform.parent.parent.GetComponent<RectTransform>();
        selfTransform.sizeDelta = new Vector2(selfTransform.rect.width, parentTransform.rect.height / 5);
    }

    public void SetContent(int num, int score, string playerName)
    {
        int minutes = score / 60;
        int seconds = score % 60;
        string minutesStr = minutes == 0 ? "" : minutes.ToString() + " min ";
        string secondsStr = seconds.ToString() + " sec ";
        content.text = string.Format("{0}. {1} - {2}", num + 1, playerName, minutesStr + secondsStr);
    }
}

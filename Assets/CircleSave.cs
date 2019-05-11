using UnityEngine;
using UnityEngine.UI;

public class CircleSave : Circle
{
    public Image greenFill;

    public override void HandleAction()
    {
        Debug.Log("Circle " + id + " was clicked! It will be destructed now\n");

        GameManager.Instance.DestroyCircle(id);
    }

    public override void CountingUpdate()
    {
        greenFill.fillAmount = Mathf.Lerp(1, 0, currentTime / lifeTime);

        if (currentTime >= lifeTime)
        {
            Debug.Log("Not clicked! Game should be finished!\n");
            Debug.Break();

            // TODO: We won't need these. Why????
            GameManager.Instance.ExplodeCircle(id);
        }
    }
}

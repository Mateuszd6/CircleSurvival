﻿using UnityEngine;
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
            Debug.Log("Save circle not clicked! Game should be finished!\n");
            GameManager.Instance.ExplodeCircle(id);
        }
    }
}

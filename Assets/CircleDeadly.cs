using UnityEngine;

public class CircleDeadly : Circle
{
    public override void HandleAction()
    {
        Debug.Log("Black circle was clicked. Game is lost!\n");
        Debug.Break();

        GameManager.Instance.ExplodeCircle(id);
    }

    public override void CountingUpdate()
    {
        if (currentTime >= lifeTime)
        {
            GameManager.Instance.DestroyCircle(id);
        }
    }
}

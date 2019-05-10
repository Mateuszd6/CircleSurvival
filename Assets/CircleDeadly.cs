using UnityEngine;

public class CircleDeadly : Circle
{
    public override void HandleAction()
    {
        Debug.Log("Black circle was clicked. Game is lost!\n");
        Debug.Break();

        
        currentTime = 0;
        circleState = CircleState.grow;
    }

    public override void CountingUpdate()
    {
        if (currentTime >= settings.expireTime)
        {
            // TODO: We won't need these. Why????
            currentTime = 0;
            circleState = CircleState.deinitializing;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<Circle> activeCircles;

    void Awake()
    {
        activeCircles = new List<Circle>();
    }

    public void SpwanCircle()
    {
        ;        
    }

    // 
    public void DestroyCircle(int circleID)
    {
        activeCircles.RemoveAll(x => x.id == circleID);
    }
}

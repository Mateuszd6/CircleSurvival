using UnityEngine;

[CreateAssetMenu(fileName = "New Circle Settings", menuName = "Circle Settings", order = 2)]
public class CircleSettings : ScriptableObject
{
    public float initTime = 0.08f; 
    public float deinitTime = 0.08f;
    public float growTime = 1f;
    public float shakeTime = 2f;
}

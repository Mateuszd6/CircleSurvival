using UnityEngine;

[CreateAssetMenu(fileName = "New Game Settings", menuName = "Game Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Range(1, 100)]
    [SerializeField]
    public int blackCircleProbability = 10;

    // TODO: Min max property drawer!
    [SerializeField]
    public int initialLifetimeLenMin = 2;

    [SerializeField]
    public int initialLifetimeLenMax = 4;
}

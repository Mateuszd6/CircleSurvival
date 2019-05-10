using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Settings", menuName = "Game Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Serializable]
    public class Level // TODO: Name it better
    {
        [SerializeField]
        public float dotLifetimeLenMin;

        [SerializeField]
        public float dotLifetimeLenMax;

        [SerializeField]
        public float minNoSpawnInterval;

        [SerializeField]
        public float maxNoSpawnInterval;

        // The moment the level ends of infinity if it does not (because of being the last).
        [SerializeField]
        public float endofTime;
    }

    [Range(1, 100)]
    [SerializeField]
    public int blackCircleProbability = 10;

    [SerializeField]
    public Level[] levels;
}

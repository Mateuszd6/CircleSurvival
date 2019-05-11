using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Settings", menuName = "Game Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Serializable]
    public class Level
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
        public float levelDuration;
    }

    [Range(0, 1)]
    [SerializeField]
    public float blackCircleProbability = 0.1f;

    [SerializeField]
    public Level[] levels;
}

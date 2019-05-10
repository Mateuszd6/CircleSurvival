using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Highscores
{
    [Serializable]
    public class HighscoreRecord
    {
        [SerializeField]
        public string playerName;

        [SerializeField]
        public int secondsSurvived;

        public HighscoreRecord(string playerName, int secondsSurvived)
        {
            this.playerName = playerName;
            this.secondsSurvived = secondsSurvived;
        }
    }

    [SerializeField]
    public List<HighscoreRecord> records;

    [SerializeField]
    public string checksum;

    public Highscores()
    {
        if (records == null)
            records = new List<HighscoreRecord>();
    }
}

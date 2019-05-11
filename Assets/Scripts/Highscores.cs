using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Highscores
{
    [Serializable]
    public class Record
    {
        [SerializeField]
        public string playerName;

        [SerializeField]
        public int secondsSurvived;

        public Record(string playerName, int secondsSurvived)
        {
            this.playerName = playerName;
            this.secondsSurvived = secondsSurvived;
        }
    }

    [SerializeField]
    public List<Record> records;

    [SerializeField]
    public string checksum;

    public Highscores()
    {
        if (records == null)
            records = new List<Record>();
    }
}

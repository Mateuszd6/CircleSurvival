using System.Collections.Generic;
using UnityEngine;

public class Ladderboard : MonoBehaviour
{
    public Ladderboardrecord recordProto;
    public Transform scrollAreaTransform;

    private List<Ladderboardrecord> records;

    void Awake()
    {
        records = new List<Ladderboardrecord>();
    }

    public void Populate()
    {
        // First we drop all records that have been here before. This will happen 
        // if the user will go back and forth on both menu screen.
        foreach (Ladderboardrecord record in records)
            Destroy(record.gameObject);
        records.Clear();

        // Now fetch the highscore table and put the results on the screen:
        Highscores hs = HighscoreManager.Instance.Highscores;
        recordProto.gameObject.SetActive(true);
        for (int i = 0; i < hs.records.Count; ++i)
        {
            Ladderboardrecord newRecord = 
                Instantiate(recordProto.gameObject, scrollAreaTransform.transform)
                    .GetComponent<Ladderboardrecord>();

            records.Add(newRecord);
            newRecord.SetContent(i, hs.records[i].secondsSurvived, hs.records[i].playerName);
        }
        recordProto.gameObject.SetActive(false);
    }
}

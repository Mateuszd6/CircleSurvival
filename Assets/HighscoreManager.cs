using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class HighscoreManager : MonoSingleton<HighscoreManager>
{
    private static readonly string highscoresFilename = "highscores.json";
    private static readonly int maxRecords = 3;

    // As MBs do not have contructors we have to initialize the path variable lazily.
    private string _highscoresFilePath;
    private string HighscoresFilePath
    {
        get
        {
            if (_highscoresFilePath == null)
            {
                _highscoresFilePath =
                    Path.Combine(Application.persistentDataPath, highscoresFilename);
            }

            return _highscoresFilePath;
        }
    }

    private Highscores _highscores;
    public Highscores Highscores
    {
        get
        {
            if (_highscores == null)
                _highscores = LoadOrCreateHighscores();
            return _highscores;
        }
    }

    // Returns MD5 sum from the highscore records.
    private string GetMd5FromRecords(List<Highscores.HighscoreRecord> highscores)
    {
        byte[] hash;

        // Use memory stream to convert list of highscores to bytes
        using (MemoryStream ms = new MemoryStream(512))
        {
            foreach (Highscores.HighscoreRecord hr in highscores)
            {
                byte[] nameAsBytes = Encoding.UTF8.GetBytes(hr.playerName);
                byte[] scoreAsBytes = BitConverter.GetBytes(hr.secondsSurvived);
                ms.Write(nameAsBytes, 0, nameAsBytes.Length);
                ms.Write(scoreAsBytes, 0, scoreAsBytes.Length);
            }

            // Now get the sum from the bytes.
            hash = new MD5CryptoServiceProvider().ComputeHash(ms.ToArray());
        }

        // Convert md5 to string so that its easier to store in json.
        StringBuilder sb = new StringBuilder();
        foreach (byte b in hash)
            sb.Append(b.ToString("x2").ToLower());
        return sb.ToString();
    }

    // Returns true if the given score will fit in the highscore ladderboard (there is 
    // a spot, or it beats some records from the list).
    public bool CheckIfHighscore(int score)
    {
        return (Highscores.records.Count < maxRecords
                || Highscores.records[maxRecords - 1].secondsSurvived <= score);
    }

    // Returns true if the score fits in the highscores (top n results). If there are already 
    // n records in the list and the added one is not the worst, the worst is removed. If current
    // highscore is same as worst and one must be removed oldest record is always deleted.
    public bool AddScoreToHighscores(string userName, int score)
    {
        // Find the first index in which the score is less, as we must insert before that 
        // score. If none is found, our record will be appended to the back.
        int insertIdx = Highscores.records.FindIndex(x => x.secondsSurvived <= score);
        if (insertIdx == -1)
            insertIdx = Highscores.records.Count;

        // Score is not in the best maxRecords.
        if (insertIdx > maxRecords - 1)
            return false;

        // Add our record, remove the worst, save to the file and tell the user, 
        // that he is in the top maxRecords.
        Highscores.records.Insert(insertIdx, new Highscores.HighscoreRecord(userName, score));
        if (Highscores.records.Count > maxRecords)
            Highscores.records.RemoveRange(maxRecords, Highscores.records.Count - maxRecords);
        SaveHighscores(Highscores);

        return true;
    }

    // Save highscores object into the file.
    private void SaveHighscores(Highscores highscores)
    {
        highscores.checksum = GetMd5FromRecords(highscores.records);

        string jsonString = JsonUtility.ToJson(highscores);
        using (StreamWriter streamWriter = File.CreateText(HighscoresFilePath))
        {
            streamWriter.Write(jsonString);
        }
    }

    // If the file with highscores exists reads it into memory when its does not 
    // (or is corrupted) creates an empty highscores file.
    private Highscores LoadOrCreateHighscores()
    {
        Highscores retval = null;

        if (File.Exists(HighscoresFilePath))
        {
            using (StreamReader streamReader = File.OpenText(HighscoresFilePath))
            {
                string jsonString = streamReader.ReadToEnd();
                retval = JsonUtility.FromJson<Highscores>(jsonString);
            }

            string md5 = GetMd5FromRecords(retval.records);
            if (md5 != retval.checksum)
            {
                Debug.LogError("Highscores file has been changed or corrupted!\n");
                retval = null;
            }
        }

        if (retval == null)
        {
            Debug.Log("Creating new Highscores file.\n");
            Highscores hs = new Highscores();

            SaveHighscores(hs);
            return hs;
        }

        return retval;
    }
}

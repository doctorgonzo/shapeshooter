using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LeaderboardEntry
{
    public string name;
    public int score;

    public LeaderboardEntry(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}

[Serializable]
public class LeaderboardSaveData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class Leaderboard : MonoBehaviour
{
    private const string SaveKey = "Leaderboard";
    private const int MaxEntries = 10;

    [SerializeField] private List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

    private Dictionary<string, int> bestScoresByName = new Dictionary<string, int>();

    private void Awake()
    {
        LoadScores();
    }

    public void AddScore(string playerName, int score)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = "Player";
        }

        playerName = playerName.Trim();

        if (bestScoresByName.ContainsKey(playerName))
        {
            if (score <= bestScoresByName[playerName])
            {
                return;
            }

            bestScoresByName[playerName] = score;

            LeaderboardEntry existingEntry = entries.Find(entry => entry.name == playerName);
            if (existingEntry != null)
            {
                existingEntry.score = score;
            }
        }
        else
        {
            bestScoresByName.Add(playerName, score);
            entries.Add(new LeaderboardEntry(playerName, score));
        }

        SortAndTrim();
        SaveScores();
    }

    public List<LeaderboardEntry> GetTopEntries()
    {
        SortAndTrim();
        return new List<LeaderboardEntry>(entries);
    }

    public bool IsHighScore(int score)
    {
        SortAndTrim();

        if (entries.Count < MaxEntries)
        {
            return true;
        }

        return score > entries[entries.Count - 1].score;
    }

    public void ClearScores()
    {
        entries.Clear();
        bestScoresByName.Clear();
        PlayerPrefs.DeleteKey(SaveKey);
    }

    private void SortAndTrim()
    {
        entries.Sort((a, b) => b.score.CompareTo(a.score));

        if (entries.Count > MaxEntries)
        {
            entries.RemoveRange(MaxEntries, entries.Count - MaxEntries);
        }
    }

    private void SaveScores()
    {
        LeaderboardSaveData saveData = new LeaderboardSaveData
        {
            entries = entries
        };

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    private void LoadScores()
    {
        entries.Clear();
        bestScoresByName.Clear();

        if (!PlayerPrefs.HasKey(SaveKey))
        {
            return;
        }

        string json = PlayerPrefs.GetString(SaveKey);
        LeaderboardSaveData saveData = JsonUtility.FromJson<LeaderboardSaveData>(json);

        if (saveData == null || saveData.entries == null)
        {
            return;
        }

        entries = saveData.entries;

        foreach (LeaderboardEntry entry in entries)
        {
            if (!bestScoresByName.ContainsKey(entry.name))
            {
                bestScoresByName.Add(entry.name, entry.score);
            }
        }

        SortAndTrim();
    }
}

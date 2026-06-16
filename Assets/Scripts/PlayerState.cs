using UnityEngine;

/// <summary>
/// Cross-scene player state. Lives, score and the run-spanning accuracy stats live here so they
/// survive scene loads without the Player GameObject having to persist (DontDestroyOnLoad).
/// Static so the values outlive any single scene's objects — same pattern as LevelSequence.
/// Health/shield/upgrades are intentionally NOT here: they reset at every level boundary.
/// </summary>
public static class PlayerState
{
    public const int StartingLives = 3;

    public static int Lives = StartingLives;
    public static int Score;
    public static float ShotsFired;
    public static float ShotsHit;

    public static float Accuracy => ShotsFired > 0f ? ShotsHit / ShotsFired : 0f;

    public static void AddScore(int amount) => Score += amount;

    public static void RemoveScore(int amount) => Score = Mathf.Max(0, Score - amount);

    public static void ResetForNewGame()
    {
        Lives = StartingLives;
        Score = 0;
        ShotsFired = 0f;
        ShotsHit = 0f;
    }
}

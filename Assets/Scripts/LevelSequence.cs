/// <summary>
/// Single source of truth for the level order and which level the player is currently on.
/// Static so the current index survives scene loads (MonoBehaviours are destroyed on load).
/// Add a scene name here (and to Build Settings) and it's part of the game — no other code changes.
/// </summary>
public static class LevelSequence
{
    // The only place level order is defined.
    public static readonly string[] Levels = { "Level1", "Level2", "Level3", "Level4" };

    public static int CurrentIndex { get; private set; } = 0;

    public static void Reset() => CurrentIndex = 0;

    /// <summary>
    /// Snap CurrentIndex to a level scene by name. Lets completion logic trust the scene you're
    /// actually in rather than a counter that drifts when a level is loaded out of order
    /// (e.g. Play-testing a single scene). No-op if the name isn't a level.
    /// </summary>
    public static void SyncTo(string sceneName)
    {
        int i = System.Array.IndexOf(Levels, sceneName);
        if (i >= 0) CurrentIndex = i;
    }

    public static bool HasNext => CurrentIndex + 1 < Levels.Length;

    public static string CurrentScene => Levels[CurrentIndex];

    public static void Advance() => CurrentIndex++;
}

/// <summary>
/// Single source of truth for the level order and which level the player is currently on.
/// Static so the current index survives scene loads (MonoBehaviours are destroyed on load).
/// Add a scene name here (and to Build Settings) and it's part of the game — no other code changes.
/// </summary>
public static class LevelSequence
{
    // The only place level order is defined.
    public static readonly string[] Levels = { "Level1", "Level2", "Level3" };

    public static int CurrentIndex { get; private set; } = 0;

    public static void Reset() => CurrentIndex = 0;

    public static bool HasNext => CurrentIndex + 1 < Levels.Length;

    public static string CurrentScene => Levels[CurrentIndex];

    public static void Advance() => CurrentIndex++;
}

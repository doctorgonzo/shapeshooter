using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The set of spawn formations EnemySpawner knows how to produce.
/// A WaveStep references one of these by value.
/// </summary>
public enum Formation
{
    Wedge,
    Circle,
    Line,
    Seeker,
    Boss
}

/// <summary>
/// One spawn within a wave: wait <see cref="delay"/> seconds, then spawn <see cref="formation"/>.
/// </summary>
[Serializable]
public class WaveStep
{
    [Tooltip("Seconds to wait before this formation spawns.")]
    public float delay;
    public Formation formation;
}

/// <summary>
/// An ordered list of spawns. The next wave begins once every enemy from this wave is dead.
/// </summary>
[Serializable]
public class Wave
{
    public List<WaveStep> steps = new List<WaveStep>();
}

/// <summary>
/// Per-level data that drives a generic <see cref="EnemySpawner"/>. Create one asset per level
/// (Assets &gt; Create &gt; ShapeShooter &gt; Level Config) and assign it to that level's spawner,
/// instead of cloning the spawner script per level.
/// </summary>
[CreateAssetMenu(fileName = "LevelConfig", menuName = "ShapeShooter/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Tooltip("Boss prefab spawned by any WaveStep whose formation is Boss.")]
    public GameObject bossPrefab;

    [Tooltip("Waves run in order. A new wave starts once all enemies from the previous wave are dead.")]
    public List<Wave> waves = new List<Wave>();
}

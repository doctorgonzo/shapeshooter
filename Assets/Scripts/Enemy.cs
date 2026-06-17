using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 2;
    [SerializeField] public int scoreValue = 100;
    private EnemySpawner enemySpawner;
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private GameObject squareExplosionPrefab;
    [SerializeField] private GameObject seekExplosionPrefab;

    private void Start()
    {
        enemySpawner = FindAnyObjectByType<EnemySpawner>(); // Find the EnemySpawner in the scene
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        if (enemySpawner != null)
        {
            // enemySpawner.enemiesAlive.Remove(gameObject); // Remove the enemy from the list of alive enemies in the EnemySpawner
            int index = gameObject.name.IndexOf("(Clone)"); // Find the index of "(Clone)" in the enemy's name
            enemySpawner.enemiesAliveNames.Remove(gameObject.name.Substring(0, index)); // Remove the enemy's name from the list of alive enemy names in the EnemySpawner
        }
        if (powerUpPrefab != null)
        {
            if (Random.value < 0.25f) // 20% chance to drop a power-up
            {
                Instantiate(powerUpPrefab, transform.position, Quaternion.identity); // Spawn a power-up at the enemy's position
            }
        }
        if (gameObject.name.Contains("Boss"))
        {
            if (Player.Instance != null)
            {
                Player.Instance.SetPlayerCollider(false);
                Player.Instance.SetPlayerControls(false);
                Player.Instance.SetPlayerVisible(false);
            }
            // Trust the scene we actually beat (not a possibly-stale counter), then let the level
            // list decide whether this boss ended a level or the whole game.
            LevelSequence.SyncTo(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(LevelSequence.HasNext ? "LevelComplete" : "YouWin");
        }
        PlayerState.AddScore(scoreValue); // Credit the player's run score when the enemy dies
        Destroy(gameObject); // Destroy the enemy game object
        if(gameObject.name.Contains("Square"))
        {
            Instantiate(squareExplosionPrefab, transform.position, Quaternion.identity);
        }
        else if (gameObject.name.Contains("Seek"))
        {
            Instantiate(seekExplosionPrefab, transform.position, Quaternion.identity);
        }
    }
}

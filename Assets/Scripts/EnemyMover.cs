using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private GameObject[] hardPoints; // Assign any number; bullets fire from each.
    private EnemySpawner enemySpawner;
    private float offScreenTimer = 0f;
    private float shootTimer = 0f;
    private float shootCooldown = 1.5f; // Time in seconds between enemy shots
    private float offScreenThreshold = 1.5f; // Time in seconds before the enemy is destroyed after moving off-screen
    private Transform playerTransform;
    private bool isBoss = false;
    private bool isSeeking = false;
    private bool shootDown = false;
    [SerializeField] private float bobSpeed = 3f;
[SerializeField] private float bobAmplitude = 2f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = Player.Instance != null ? Player.Instance.transform : null; // Player is scene-local; may be absent
        if (gameObject.name.Contains("Boss"))
        {
            isBoss = true; // Set isBoss to true if the enemy is a boss
            GetComponent<Enemy>().scoreValue = 4200;
        }
        if (gameObject.name.Contains("Seeking"))
        {
            isSeeking = true;
            GetComponent<Enemy>().scoreValue = 250;
        }
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        
    }
    private void Start()
    {
        // rb.linearVelocity = Vector2.left * moveSpeed; // Move the enemy to the left at the specified speed
        enemySpawner = FindAnyObjectByType<EnemySpawner>(); // Find the EnemySpawner in the scene
    }
    private void FixedUpdate()
    {
        if (isBoss)
        {
            CirclePlayer();
            offScreenThreshold = 100f;
        }
        else if (!isSeeking)
        {
            float bobVelocity = Mathf.Cos(Time.fixedTime * bobSpeed) * bobAmplitude;
            rb.linearVelocity = new Vector2(-moveSpeed, bobVelocity);
        }
        else if (isSeeking)
        {
            CirclePlayer();
            offScreenThreshold = 12;
        }
    }
    private void CirclePlayer()
    {
        //approach to within range
        Vector2 directionToPlayer = playerTransform.position - transform.position;
        if (directionToPlayer.magnitude > 5f) // If we're farther than 5 units from the player, move towards them
        {
            Vector2 moveDirection = directionToPlayer.normalized;
            rb.linearVelocity = moveDirection * moveSpeed;
        }
        else // If we're within 5 units of the player, circle around them
        {
            Vector2 perpendicularDirection = new Vector2(-directionToPlayer.y, directionToPlayer.x).normalized; // Perpendicular to the direction to the player
            rb.linearVelocity = perpendicularDirection * moveSpeed; // Move in the perpendicular direction to circle around the player
            //rotate towards the player
            transform.rotation = Quaternion.LookRotation(Vector3.forward, directionToPlayer);
        }
    }
    // The transforms to fire from: every assigned hardpoint, or this enemy's own transform as a
    // fallback so it still shoots even with nothing wired up. Never returns an empty list.
    private List<Transform> GetFirePoints()
    {
        List<Transform> points = new List<Transform>();
        if (hardPoints != null)
        {
            foreach (GameObject hp in hardPoints)
            {
                if (hp != null) points.Add(hp.transform);
            }
        }
        if (points.Count == 0) points.Add(transform);
        return points;
    }

    private void Shoot()
    {
        Transform firePoint = GetFirePoints()[0];
        if (gameObject.name.Contains("Seek") || gameObject.name.Contains("Boss"))
        {
            GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, quaternion.Euler(0,0,90));
            bullet.GetComponent<BulletMover>().isSeeking = true;
        }
        Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.Euler(0,0,90));
    }
    private void ShootDown()
    {
        // //aim towards the player
        // Vector2 directionToPlayer = playerTransform.position - transform.position;
        // float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        // Instantiate(enemyBulletPrefab, hardPoint1.transform.position, Quaternion.Euler(0, 0, angle-90));
        // // Debug.Log("Shooting down at the player!");
    }
    private void ShootBoss()
    {
        shootCooldown = 0.75f;
        StartCoroutine(ShootBossCoroutine());
    }
    private IEnumerator ShootBossCoroutine()
{
    foreach (Transform hardPoint in GetFirePoints())
    {
        // Re-aim per shot so each bullet leads toward where the player is right now,
        // not where they were when the volley started (shots are staggered).
        Vector2 directionToPlayer = playerTransform != null ? (Vector2)(playerTransform.position - hardPoint.position) : Vector2.up;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        GameObject bullet = Instantiate(enemyBulletPrefab, hardPoint.position, Quaternion.Euler(0, 0, angle - 90));
        bullet.GetComponent<BulletMover>().isSeeking = true;
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.5f)); // Random delay between shots for a more dynamic pattern
    }
}
    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 30f, LayerMask.GetMask("Player"));
        if (hit.collider != null)
        {            if (hit.collider.CompareTag("Player"))
            {
                shootDown = true;
            }
        }
        CheckIfOffScreen(); // Check if the enemy has moved off-screen
        shootTimer -= Time.deltaTime; // Decrease the shoot timer by the time that has passed since the last frame
        if (shootTimer <= 0f)
    {
        if (isBoss)
            {
            ShootBoss();
            }
        else
            {
            Shoot();
            }
        if (shootDown)
        {
            ShootDown();
        }

        shootTimer = shootCooldown;
    }
    // Keep the cached player transform current (it's scene-local and may respawn/disappear).
    if (Player.Instance != null) playerTransform = Player.Instance.transform;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("RearBoundary"))
        {
            if (enemySpawner != null)
            {
                int index = gameObject.name.IndexOf("(Clone)"); // Find the index of "(Clone)" in the enemy's name
                enemySpawner.enemiesAliveNames.Remove(gameObject.name.Substring(0, index)); // Remove the enemy's name from the list of alive enemy names in the EnemySpawner
            }
            // Handle collision with the rear boundary (e.g., destroy the enemy)
            Destroy(gameObject); // Destroy the enemy on collision with the rear boundary
        }
    }
    private void CheckIfOffScreen()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
        {
            // Debug.Log("The object has moved off-screen!");
            offScreenTimer += Time.deltaTime; // Increment the timer while the enemy is off-screen
            if (offScreenTimer >= offScreenThreshold)
            {
                if (enemySpawner != null)
                {
                    // enemySpawner.enemiesAlive.Remove(gameObject); // Remove the enemy from the list of alive enemies in the EnemySpawner
                    int index = gameObject.name.IndexOf("(Clone)"); // Find the index of "(Clone)" in the enemy's name
                    enemySpawner.enemiesAliveNames.Remove(gameObject.name.Substring(0, index)); // Remove the enemy's name from the list of alive enemy names in the EnemySpawner
                }
                PlayerState.RemoveScore(50);
                Destroy(gameObject); // Destroy the enemy if it has moved off-screen
            }
        }
        else
        {
            offScreenTimer = 0f; // Reset the timer if the enemy is back on-screen
        }
    }
}

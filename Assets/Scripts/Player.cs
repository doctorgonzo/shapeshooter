using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
   [SerializeField] public int maxHealth = 4;
   [SerializeField] public int maxLives = 3;
   public int health;
   public int lives;
   [SerializeField] public int maxShield = 5;
   public int shield;
   [SerializeField] private SpriteRenderer shieldSpriteRenderer;
   [SerializeField] private TMPro.TextMeshProUGUI livesText;
   [SerializeField] private GameObject[] lifeIcons;
   [SerializeField] private Transform respawnPoint;
   [SerializeField, Min(0.01f)] private float deathFlashDuration = 1.2f;
   [SerializeField, Min(0.01f)] private float flashInterval = 0.12f;
   private float shieldRegenStart = 3.5f; // Time in seconds between taking damage
    private float damageTimer = 0f;
    private PlayerController playerController;
    private Rigidbody2D playerRB;
    private Collider2D playerCollider;
    private SpriteRenderer playerSpriteRenderer;
    private Vector3 startingPosition;
    private Quaternion startingRotation;
    public bool isDying;
    public float shotsFired;
    public float shotsHit;
    public float accuracy;
    public static Player Instance {get; private set;}
   private void Awake()
   {
    if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // Set the global reference to this instance
        Instance = this;
       // Optional: Keep this object alive across scene transitions
       DontDestroyOnLoad(gameObject);
       playerController = GetComponent<PlayerController>();
       playerRB = GetComponent<Rigidbody2D>();
       playerCollider = GetComponent<Collider2D>();
       playerSpriteRenderer = GetComponent<SpriteRenderer>();
       startingPosition = transform.position;
       startingRotation = transform.rotation;
       health = maxHealth;
       lives = maxLives;
       shield = maxShield;
       UpdateShieldVisual();
       UpdateLivesVisual();
   }
    private void Update()
    {
        if (isDying)
        {
            return;
        }
        if (shotsFired > 0)
        {
            accuracy = shotsHit / shotsFired;
        }
        damageTimer += Time.deltaTime;
        if (shield < maxShield && damageTimer >= shieldRegenStart)
        {
            shield += 1; // Regenerate 1 shield point
            UpdateShieldVisual();
            damageTimer = 0f; // Reset the damage timer after regenerating a shield point
        }
        if (health == maxHealth)
        {
            playerSpriteRenderer.color = Color.limeGreen;
        }
        else if (health == 3)
        {
            playerSpriteRenderer.color = Color.greenYellow;
        }
        else if (health == 2)
        {
            playerSpriteRenderer.color = Color.yellow;
        }
        else if (health == 1)
        {
            playerSpriteRenderer.color = Color.orangeRed;
        }
    }
    private void OnLevelWasLoaded(int level)
    {
        lifeIcons[0] = GameObject.Find("LivesDisplay1");
        lifeIcons[1] = GameObject.Find("LivesDisplay2");
        lifeIcons[2] = GameObject.Find("LivesDisplay3");
        UpdateLivesVisual();
    }
    public void TakeDamage(int damage)
   {
       if (isDying)
       {
           return;
       }

       if (shield > 0)
       {
           int shieldDamage = Mathf.Min(shield, damage);
           shield -= shieldDamage;
           damage -= shieldDamage;
           UpdateShieldVisual();
           damageTimer = 0f; // Reset the damage timer when taking damage
       }

       if (damage > 0)
       {
           health -= damage;
           damageTimer = 0f; // Reset the damage timer when taking damage
           if (health <= 0)
           {
               Die();
           }
       }
   }
   private void UpdateShieldVisual()
   {
       if (shieldSpriteRenderer == null)
       {
           return;
       }

       if (shield >= 4)
       {
           shieldSpriteRenderer.enabled = true;
           Color color = shieldSpriteRenderer.color;
           color.a = 0.78f; // Full opacity
           shieldSpriteRenderer.color = color;
       }
       else if (shield == 3)
       {
           shieldSpriteRenderer.enabled = true;
           Color color = shieldSpriteRenderer.color;
           color.a = 0.5f; // Semi-transparent
           shieldSpriteRenderer.color = color;
       }
       else if (shield == 2)
       {
           shieldSpriteRenderer.enabled = true;
           Color color = shieldSpriteRenderer.color;
           color.a = 0.25f; // More transparent
           shieldSpriteRenderer.color = color;
       }
       else if (shield == 1)
       {
           shieldSpriteRenderer.enabled = true;
           Color color = shieldSpriteRenderer.color;
           color.a = 0.1f; // Very transparent
           shieldSpriteRenderer.color = color;
       }
       else
       {
           shieldSpriteRenderer.enabled = false;
       }
   }

   public void ApplyPowerUp(PowerUpType type)
   {
       switch (type)
       {
           case PowerUpType.Health:
               AddHealth();
               break;
           case PowerUpType.Shield:
               AddShield();
               break;
           case PowerUpType.RapidFire:
               ActivateRapidFire();
               break;
           case PowerUpType.AddGun:
               AddGun();
               break;
       }
   }

   public void AddShield(int amount = 1)
    {
        shield = Mathf.Min(shield + amount, maxShield);
        UpdateShieldVisual();
    }

    public void AddHealth(int amount = 1)
    {
        health = Mathf.Min(health + amount, maxHealth);
    }

    private void ActivateRapidFire()
    {
        if (playerController != null)
        {
            playerController.ActivateRapidFire();
        }
    }

    private void AddGun()
    {
        if (playerController != null)
        {
            playerController.AddGun();
        }
    }

   private void Die()
   {
       if (isDying)
       {
           return;
       }

       Debug.Log("Player has died!");
       isDying = true;
       lives = Mathf.Max(0, lives - 1);
       UpdateLivesVisual();
       StartCoroutine(DeathRoutine());
   }

   private IEnumerator DeathRoutine()
   {
       SetPlayerControls(false);
       SetPlayerCollider(false);
       StopPlayerMovement();

       yield return FlashPlayer();

       if (lives <= 0)
       {
           SetPlayerVisible(false);
           Debug.Log("Game over!");
           SceneManager.LoadScene("GameOver");
           yield break;
       }

       ResetPlayer();
       SetPlayerVisible(true);
       SetPlayerCollider(true);
       SetPlayerControls(true);
       isDying = false;
   }

   private IEnumerator FlashPlayer()
   {
       float elapsed = 0f;
       bool visible = false;

       while (elapsed < deathFlashDuration)
       {
           visible = !visible;
           SetPlayerVisible(visible);
           yield return new WaitForSeconds(flashInterval);
           elapsed += flashInterval;
       }

       SetPlayerVisible(true);
   }

   public void ResetPlayer()
   {
       health = maxHealth;
       shield = maxShield;
       damageTimer = 0f;
       UpdateShieldVisual();

       transform.position = respawnPoint != null ? respawnPoint.position : startingPosition;
       transform.rotation = respawnPoint != null ? respawnPoint.rotation : startingRotation;
       StopPlayerMovement();
   }
   public void RestartPlayer()
    {
            damageTimer = 0;
            shotsFired = 0;
            shotsHit = 0;
            accuracy = 0;
            lives = maxLives;
            health = maxHealth;
            shield = maxShield;
            SetPlayerCollider(true);
            SetPlayerControls(true);
            SetPlayerVisible(true);
            isDying = false;
            UpdateLivesVisual();
            UpdateShieldVisual();
            ScoreKeeper.Instance.ResetScore();
            transform.position = respawnPoint != null ? respawnPoint.position : startingPosition;
            transform.rotation = respawnPoint != null ? respawnPoint.rotation : startingRotation;
            StopPlayerMovement();
    }
   public void UpdateLivesVisual()
   {
       if (livesText != null)
       {
           livesText.text = "Lives: " + lives;
       }

       if (lifeIcons == null)
       {
           return;
       }

       for (int i = 0; i < lifeIcons.Length; i++)
       {
           if (lifeIcons[i] != null)
           {
               lifeIcons[i].SetActive(i < lives);
           }
       }
   }

   public void SetPlayerControls(bool enabled)
   {
       if (playerController != null)
       {
           playerController.enabled = enabled;
       }
   }

   public void SetPlayerCollider(bool enabled)
   {
       if (playerCollider != null)
       {
           playerCollider.enabled = enabled;
       }
   }

   public void SetPlayerVisible(bool visible)
   {
       if (playerSpriteRenderer != null)
       {
           Color color = playerSpriteRenderer.color;
           color.a = visible ? 1f : 0f;
           playerSpriteRenderer.color = color;
       }
   }

   private void StopPlayerMovement()
   {
       if (playerRB != null)
       {
           playerRB.linearVelocity = Vector2.zero;
           playerRB.angularVelocity = 0f;
       }
   }
}

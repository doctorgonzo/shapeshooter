using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PowerUp : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool collected;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private TextMeshProUGUI powerUpText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool randomizeOnSpawn = true;
    [SerializeField] private PowerUpType selectedType = PowerUpType.Health;
    [SerializeField] private PowerUpOption[] powerUpOptions =
    {
        new PowerUpOption(PowerUpType.Health, "H", new Color(0.35f, 1f, 0.45f), 1f),
        new PowerUpOption(PowerUpType.Shield, "S", new Color(0.35f, 0.75f, 1f), 1f),
        new PowerUpOption(PowerUpType.RapidFire, "R", new Color(1f, 0.82f, 0.25f), 1f),
        new PowerUpOption(PowerUpType.AddGun, "G", new Color(1f, 0.45f, 0.9f), 1f)
    };

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        selectedType = randomizeOnSpawn ? ChooseWeightedType() : selectedType;
        UpdateVisuals();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = Vector2.left * moveSpeed;
        Vector2 directionToPlayer = Player.Instance.transform.position - transform.position;
        if (directionToPlayer.magnitude < 5f && directionToPlayer.magnitude > 0.1f) // If we're farther than 5 units from the player, move towards them
        {
            Vector2 moveDirection = directionToPlayer.normalized;
            rb.linearVelocity = moveDirection * moveSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player == null)
        {
            player = collision.GetComponentInParent<Player>();
        }

        Collect(player);
    }

    public void Collect(Player player)
    {
        if (collected || player == null)
        {
            return;
        }

        collected = true;
        player.ApplyPowerUp(selectedType);
        Destroy(gameObject);
    }

    private PowerUpType ChooseWeightedType()
    {
        if (powerUpOptions == null || powerUpOptions.Length == 0)
        {
            return selectedType;
        }

        float totalWeight = 0f;
        for (int i = 0; i < powerUpOptions.Length; i++)
        {
            if (powerUpOptions[i] != null && powerUpOptions[i].weight > 0f)
            {
                totalWeight += powerUpOptions[i].weight;
            }
        }

        if (totalWeight <= 0f)
        {
            return selectedType;
        }

        float roll = Random.value * totalWeight;
        for (int i = 0; i < powerUpOptions.Length; i++)
        {
            PowerUpOption option = powerUpOptions[i];
            if (option == null || option.weight <= 0f)
            {
                continue;
            }

            roll -= option.weight;
            if (roll <= 0f)
            {
                return option.type;
            }
        }

        return selectedType;
    }

    private void UpdateVisuals()
    {
        PowerUpOption option = FindOption(selectedType);

        if (powerUpText != null)
        {
            powerUpText.text = option != null ? option.label : selectedType.ToString();
        }

        if (spriteRenderer != null && option != null)
        {
            spriteRenderer.color = option.color;
        }
    }

    private PowerUpOption FindOption(PowerUpType type)
    {
        if (powerUpOptions == null)
        {
            return null;
        }

        for (int i = 0; i < powerUpOptions.Length; i++)
        {
            if (powerUpOptions[i] != null && powerUpOptions[i].type == type)
            {
                return powerUpOptions[i];
            }
        }

        return null;
    }

    [System.Serializable]
    private class PowerUpOption
    {
        public PowerUpType type;
        public string label;
        public Color color;
        [Min(0f)] public float weight;

        public PowerUpOption()
        {
            type = PowerUpType.Health;
            label = "H";
            color = Color.white;
            weight = 1f;
        }

        public PowerUpOption(PowerUpType type, string label, Color color, float weight)
        {
            this.type = type;
            this.label = label;
            this.color = color;
            this.weight = weight;
        }
    }
}

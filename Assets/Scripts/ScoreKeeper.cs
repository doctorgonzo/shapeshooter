using JetBrains.Annotations;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static ScoreKeeper Instance { get; private set; }
    [SerializeField] private int score = 0;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    private void Awake()
    {
        // If an instance already exists and it's not this, destroy this duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // Set the global reference to this instance
        Instance = this;
        // Optional: Keep this object alive across scene transitions
        DontDestroyOnLoad(gameObject);
        scoreText = GameObject.Find("ScoreText").GetComponent<TMPro.TextMeshProUGUI>();
    }
    private void Update()
    {
        scoreText.text = "Score: " + score.ToString();
    }
    private void OnLevelWasLoaded(int level)
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<TMPro.TextMeshProUGUI>();
    }
    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
    }
    public void RemoveScore(int scoreToRemove)
    {
        score -= scoreToRemove;
        if (score < 0)
        {
            score = 0; // Ensure score doesn't go negative
        }
    }
    public int GetScore()
    {        
        return score;
    }

    public void ResetScore()
    {
        score = 0;
    }
}

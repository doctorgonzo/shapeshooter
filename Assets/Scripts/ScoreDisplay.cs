using TMPro;
using UnityEngine;

/// <summary>
/// Scene-local score readout. Reads PlayerState.Score (the single source of truth) and pushes it
/// to a wired-up text field. Replaces the old ScoreKeeper singleton — drop one on the score text
/// in each gameplay scene and wire the field in the inspector.
/// </summary>
public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (scoreText == null)
        {
            scoreText = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + PlayerState.Score;
        }
    }
}

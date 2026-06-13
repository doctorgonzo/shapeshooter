using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YouWin : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private Leaderboard leaderboard;
    [SerializeField] private TextMeshProUGUI[] entries;

    private int finalScore;
    private bool submittedScore = false;

    private void Awake()
    {
        if (nameInput == null)
        {
            GameObject nameInputObject = GameObject.Find("NameEntry");
            if (nameInputObject != null)
            {
                nameInput = nameInputObject.GetComponent<TMP_InputField>();
            }
        }

        if (submitButton == null)
        {
            GameObject submitButtonObject = GameObject.Find("NameEntryButton");
            if (submitButtonObject != null)
            {
                submitButton = submitButtonObject.GetComponent<Button>();
            }
        }

        if (submitButton != null)
        {
            submitButton.onClick.RemoveListener(SubmitScore);
            submitButton.onClick.AddListener(SubmitScore);
        }
    }

    private void Start()
    {
        finalScore = ScoreKeeper.Instance.GetScore();
        scoreText.text = "Final Score: " + finalScore;
        RefreshLeaderboardDisplay();
        SetScoreSubmissionVisible(leaderboard != null && leaderboard.IsHighScore(finalScore));
    }

    public void SubmitScore()
    {
        if (submittedScore || leaderboard == null || !leaderboard.IsHighScore(finalScore))
        {
            return;
        }

        string playerName = "Player";

        if (nameInput != null && !string.IsNullOrWhiteSpace(nameInput.text))
        {
            playerName = nameInput.text;
        }

        leaderboard.AddScore(playerName, finalScore);
        submittedScore = true;

        SetScoreSubmissionVisible(false);
        RefreshLeaderboardDisplay();
    }

    private void SetScoreSubmissionVisible(bool visible)
    {
        if (nameInput != null)
        {
            nameInput.gameObject.SetActive(visible);

            if (visible)
            {
                nameInput.ActivateInputField();
            }
        }

        if (submitButton != null)
        {
            submitButton.gameObject.SetActive(visible);
        }
    }

    private void RefreshLeaderboardDisplay()
    {
        var topEntries = leaderboard.GetTopEntries();

        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i] == null)
            {
                continue;
            }

            if (i < topEntries.Count)
            {
                LeaderboardEntry entry = topEntries[i];
                entries[i].text = $"{i + 1}. {entry.name} - {entry.score}";
            }
            else
            {
                entries[i].text = $"{i + 1}. ---";
            }
        }
    }
}

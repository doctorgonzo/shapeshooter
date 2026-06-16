using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOver : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI shotsHit;
    [SerializeField] private TMPro.TextMeshProUGUI shotsFired;
    [SerializeField] private TMPro.TextMeshProUGUI accuracy;
    [SerializeField] private TMPro.TextMeshProUGUI restartText;
    private bool isFlashing = false;

    private void Update()
    {
        DoFlashText();
        shotsFired.text = "Shots Fired: " + PlayerState.ShotsFired.ToString();
        shotsHit.text = "Shots Hit: " + PlayerState.ShotsHit.ToString();
        accuracy.text = "Total Accuracy: " + PlayerState.Accuracy.ToString("P");
        if (SceneManager.GetActiveScene().name == "LevelComplete")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // No live player in this menu scene — the next level spawns a fresh ship.
                LevelSequence.Advance();
                SceneManager.LoadScene(LevelSequence.CurrentScene);
            }
        }
    }
    private void DoFlashText()
{
    if (!isFlashing)
        StartCoroutine(FlashyText());
}

private IEnumerator FlashyText()
{
    isFlashing = true;
    float speed = 2f; // full oscillations per second
    float elapsed = 0f;

    while (true)
    {
        elapsed += Time.deltaTime;

        // Sine produces -1..1, remap to 0..1, then to your desired alpha range
        float t = (Mathf.Sin(elapsed * Mathf.PI * speed) + 1f) * 0.5f;
        float alpha = Mathf.Lerp(0.1f, 1f, t);

        Color c = restartText.color;
        c.a = alpha;
        restartText.color = c;

        yield return null; // advance one frame at a time
    }
}

}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FlashText : MonoBehaviour
{
    private TMPro.TextMeshProUGUI restartText;
    private bool isFlashing = false;

    private void Awake()
    {
        restartText = GetComponent<TMPro.TextMeshProUGUI>();
    }

    private void Update()
{
    DoFlashText();
    if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerState.ResetForNewGame();
            LevelSequence.Reset();
            SceneManager.LoadScene(LevelSequence.CurrentScene);
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

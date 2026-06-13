using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
    public void HandleStartClicked()
    {
        if (ScoreKeeper.Instance != null)
        {
        ScoreKeeper.Instance.RemoveScore(ScoreKeeper.Instance.GetScore());
        }
        SceneManager.LoadScene("Level1");
    }
    public void HandleQuitClicked()
    {
        Application.Quit();
    }
}

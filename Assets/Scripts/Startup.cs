using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
    public void HandleStartClicked()
    {
        if (Player.Instance != null)
        {
            Player.Instance.RestartPlayer();
            Player.Instance.ResetPlayer();
        }
        LevelSequence.Reset();
        SceneManager.LoadScene(LevelSequence.CurrentScene);
    }
    public void HandleQuitClicked()
    {
        Application.Quit();
    }
}

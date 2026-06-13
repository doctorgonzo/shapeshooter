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
        SceneManager.LoadScene("Level1");
    }
    public void HandleQuitClicked()
    {
        Application.Quit();
    }
}

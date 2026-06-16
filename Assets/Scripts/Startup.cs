using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
    public void HandleStartClicked()
    {
        PlayerState.ResetForNewGame();
        LevelSequence.Reset();
        SceneManager.LoadScene(LevelSequence.CurrentScene);
    }
    public void HandleQuitClicked()
    {
        Application.Quit();
    }
}

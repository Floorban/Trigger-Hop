using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    private static float gameTime;
    public bool isPaused;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        if (!isPaused) gameTime = 1;
        else gameTime = 0;
    }
    public void NextLevel() => SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    public float GetGameTime()
    {
        return gameTime;
    }
    public void ToggleGameState()
    {
        isPaused = !isPaused;
    }
}

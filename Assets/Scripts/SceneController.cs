using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    [Header("Global Time Control")]
    [SerializeField] private bool isPaused = false;
    [SerializeField][Range(0f, 1f)] private float playerTimeScale = 1f;
    [SerializeField][Range(0f, 1f)] private float enemyTimeScale = 1f;

    public bool IsPaused => isPaused;
    public float GameTime => isPaused ? 0f : 1f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitTimeScales();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void NextLevel() => SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    private void InitTimeScales()
    {
        playerTimeScale = 1f;
        enemyTimeScale = 1f;
    }

    public void ToggleGameState()
    {
        isPaused = !isPaused;
    }

    public void PauseGame() => isPaused = true;
    public void ResumeGame() => isPaused = false;

    public float GetPlayerTime() => GameTime * playerTimeScale;
    public float GetEnemyTime() => GameTime * enemyTimeScale;

    public float GetScaledTime(float scale) => GameTime * scale;
}

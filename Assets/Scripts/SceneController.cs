using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    [Header("Global Time Control")]
    [SerializeField] private bool isPaused = false;
    private float playerTimeScale = 1f;
    private float enemyTimeScale = 1f;
    [SerializeField] [Range(0f, 1f)] private float timeScale = 1f;

    public bool IsPaused => isPaused;
    public float GameTime => isPaused ? 0f : 1f;

    public float TimeScale
    {
        get => timeScale;
        set
        {
            timeScale = Mathf.Clamp01(value);
            SetScaledTime(1f);
        }
    }
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

    public float GetPlayerTime() => GameTime * playerTimeScale * timeScale;
    public float GetEnemyTime() => GameTime * enemyTimeScale * timeScale;
    public void SetScaledTime(float scale)
    {
        Time.timeScale = timeScale * scale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    } 
}

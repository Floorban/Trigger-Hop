using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    public PlayerController player;

    [Header("UI")]
    public RectTransform ammoUI;
    public GameObject finalScreen;

    [Header("Global Time Control")]
    [SerializeField] private bool isPaused = false;
    private float playerTimeScale = 1f;
    private float enemyTimeScale = 1f;
    [SerializeField] [Range(0f, 1.5f)] private float timeScale = 1f;

    public bool IsPaused => isPaused;
    public float GameTime => isPaused ? 0f : 1f;

    public float TimeScale
    {
        get => timeScale;
        set
        {
            timeScale = Mathf.Clamp(timeScale,0f,1.5f);
            SetScaledTime();
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitTimeScales();
            LevelEnd.OnLevelFinished += FinalScreen;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void FinalScreen(LevelEnd end)
    {
        CameraLock(end.lookAt);
    }
    private void CameraLock(Transform lookAt)
    {
        CameraTarget cameraTarget = new CameraTarget
        {
            TrackingTarget = lookAt,
            LookAtTarget = lookAt,
        };

        var weaponManager = FindFirstObjectByType<WeaponManager>();
        var cam = FindFirstObjectByType<CinemachineCamera>();

        weaponManager.inputLocked = true;
        cam.Target = cameraTarget;

        float targetSize = 5f;
        float duration = 2f;

        DOTween.Kill(cam);
        DOTween.To(
            () => cam.Lens.OrthographicSize,
            x => cam.Lens.OrthographicSize = x,
            targetSize,
            duration
        ).SetEase(Ease.InOutQuad)
         .SetTarget(cam);
    }
    public void NextLevel()
    {
        foreach (Transform child in ammoUI)
        {
            Destroy(child.gameObject);
        }
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
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
    public void SetScaledTime(float scale = 1f)
    {
        Time.timeScale = timeScale * scale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    } 
}

using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    public PlayerController player;
    public CameraController cam;
    public bool inLevel;

    [Header("UI")]
    public RectTransform ammoUI;
    public int numOfCoin;
    public TextMeshProUGUI coinText, timerText;
    public GameObject finalScreen;
    public TextMeshProUGUI currentLevelText;

    [Header("Global Time Control")]
    public float currentTime;
    [SerializeField] private bool isPaused = false;
    private float playerTimeScale = 1f;
    private float enemyTimeScale = 1f;
    [SerializeField] [Range(0f, 1.5f)] private float timeScale = 1f;

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
            finalScreen.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        if (isPaused) return;

        currentTime += Time.fixedDeltaTime;
        timerText.text = currentTime.ToString("0.00");
    }
    public void LevelStarted(PlayerController p)
    {
        inLevel = true;
        player = p;
        numOfCoin = 0;
        CoinCollected(0);
        currentTime = 0;
        isPaused = false;
        coinText.color = Color.white;
        coinText.color = Color.white;

    }
    public void LevelFinished(LevelEnd end)
    {
        inLevel = false;
        isPaused = true;
        player.Stop();
        FindFirstObjectByType<WeaponManager>().StopAiming();

        currentLevelText.text = "Level " + SceneManager.GetActiveScene().buildIndex;
        CameraLock(end);
    }
    private void CameraLock(LevelEnd end)
    {
        var weaponManager = FindFirstObjectByType<WeaponManager>();

        weaponManager.inputLocked = true;
        cam.target = end.lookAt;

        float targetSize = 5f;
        float duration = 1f;

        DOTween.Kill(cam);
        DOTween.To(
            () => cam.Size,
            x => cam.Size = x,
            targetSize,
            duration
        ).SetEase(Ease.InOutQuad)
         .SetTarget(cam)
         .OnComplete(() =>
         {
             finalScreen.SetActive(true);
             finalScreen.transform.localScale = Vector3.one;
             finalScreen.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 5, 0.5f);
             DOVirtual.DelayedCall(0.3f, () => TextEffects(end));
         });
    }
    private void TextEffects(LevelEnd end)
    {
        if (currentTime <= end.timeRequirement)
        {
            timerText.color = Color.green;
            timerText.rectTransform.DOPunchScale(Vector3.one * 0.3f, 0.5f, 5, 1f);
        }
        else
        {
            timerText.color = Color.red;
            timerText.rectTransform.DOShakePosition(0.3f, strength: new Vector3(5f, 0f, 0f));
            timerText.rectTransform.DOShakeRotation(0.4f, strength: 30f);

            cam.Shake();
        }

        if (numOfCoin == end.coinRequirement)
        {
            coinText.color = Color.green;
            coinText.rectTransform.DOPunchScale(Vector3.one * 0.3f, 0.5f, 5, 1f);
        }
        else
        {
            coinText.color = Color.red;
            coinText.rectTransform.DOShakeRotation(0.4f, strength: 30f);
            cam.Shake();
        }
    }
    public void NextLevel(bool next)
    {
        player = null;
        foreach (Transform child in ammoUI)
        {
            Destroy(child.gameObject);
        }
        finalScreen.SetActive(false);

        if (next)
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    public void BackToMenu()
    {
        player = null;
        SceneManager.LoadSceneAsync(0);
        Destroy(gameObject);
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
    private void CoinCollected(int amount)
    {
        numOfCoin += amount;
        coinText.text = numOfCoin.ToString();
    }
    public int GetCoins() => numOfCoin;
    public void ConsumeCoins(int amount) => numOfCoin -= amount;
    private void OnEnable()
    {
        LevelEnd.OnLevelFinished += LevelFinished;
        PlayerController.OnLevelStarted += LevelStarted;
        Coin.OnCoinCollected += CoinCollected;
    }
    private void OnDisable()
    {
        LevelEnd.OnLevelFinished -= LevelFinished;
        PlayerController.OnLevelStarted -= LevelStarted;
        Coin.OnCoinCollected -= CoinCollected;
    }
}

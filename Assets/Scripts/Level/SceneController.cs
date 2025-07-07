using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    public AudioManager audioManager;
    public PlayerController player;
    [HideInInspector] public WeaponManager weaponManager;
    public CameraController cam;
    public bool inLevel;

    [Header("UI")]
    public RectTransform ammoUI;
    public Animator timerAnim;
    public int numOfCoin;
    public TextMeshProUGUI coinText, timerText;
    public GameObject finalScreen, pauseScreen;
    public TextMeshProUGUI currentLevelText1, currentLevelText2;
    [SerializeField] private Toggle aimDir, autoReload;

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
            pauseScreen.SetActive(false);
            aimDir.onValueChanged.AddListener(ToggleAimDir);
            autoReload.onValueChanged.AddListener(ToggleAutoReload);
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
    public void PauseMenu()
    {
        if (isPaused) return;

        audioManager.PlaySfx(audioManager.spin);
        PauseGame();
        weaponManager.inputLocked = true;
        pauseScreen.SetActive(true);
        pauseScreen.transform.localScale = Vector3.zero;
        pauseScreen.transform
          .DOScale(Vector3.one, 0.5f)
          .SetEase(Ease.OutBack)
          .SetUpdate(true);
    }
    public void Unpause()
    {
        if (!isPaused) return;

        ResumeGame();
        audioManager.PlaySfx(audioManager.btnConfirm);

        pauseScreen.transform
          .DOScale(Vector3.zero, 0.2f)
          .SetEase(Ease.InOutQuad)
          .SetUpdate(true)
          .OnComplete(() =>
            {
                player.GetComponentInChildren<WeaponManager>().inputLocked = false;
                pauseScreen.SetActive(false);
            });
    }
    public void LevelStarted(PlayerController p)
    {
        weaponManager = p.GetComponentInChildren<WeaponManager>();
        audioManager.PlaySfx(audioManager.gameStart);
        inLevel = true;
        player = p;
        numOfCoin = 0;
        CoinCollected(0);
        currentTime = 0;
        isPaused = false;
        coinText.color = Color.white;
        coinText.color = Color.white;
        ToggleAimDir(aimDir.isOn);
        ToggleAutoReload(autoReload.isOn);
    }
    public void LevelFinished(LevelEnd end)
    {
        audioManager.PlaySfx(audioManager.lvlFinished);
        inLevel = false;
        isPaused = true;
        player.Stop();
        FindFirstObjectByType<WeaponManager>().StopAiming();

        currentLevelText1.text = "Level " + SceneManager.GetActiveScene().buildIndex;
        currentLevelText2.text = "Level " + SceneManager.GetActiveScene().buildIndex;
        timerAnim.SetBool("LevelEnd", true);
        CameraLock(end);
    }
    private void CameraLock(LevelEnd end)
    {
        var weaponManager = FindFirstObjectByType<WeaponManager>();

        weaponManager.inputLocked = true;
        cam.target = end.lookAt;

        float targetSize = 4f;
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
        audioManager.PlaySfx(audioManager.btnConfirm);
        player = null;
        if (ammoUI.GetChild(0))
        {
            foreach (Transform child in ammoUI)
            {
                Destroy(child.gameObject);
            }
        }

        finalScreen.SetActive(false);

        if (next)
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    public void BackToMenu()
    {
        ResumeGame();
        audioManager.PlaySfx(audioManager.btnConfirm);
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
    public void PauseGame()
    {
         isPaused = true;
         SetScaledTime(0f);
    }
    public void ResumeGame()
    {
         isPaused = false;
         SetScaledTime(1f);
    }
    public float GetPlayerTime() => GameTime * playerTimeScale * timeScale;
    public float GetEnemyTime() => GameTime * enemyTimeScale * timeScale;
    public void SetScaledTime(float scale = 1f)
    {
        Time.timeScale = timeScale * scale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
    public void ToggleAimDir(bool toggled)
    {
        foreach (var gun in weaponManager.allGuns)
        {
            if (toggled)
            {
                gun.reverseAimDir = false;
                gun.inputAimDIr = 1;
            }
            else
            {
                gun.reverseAimDir = true;
                gun.inputAimDIr = -1;
            }
        }
    }
    public void ToggleAutoReload(bool toggled)
    {
        foreach (var gun in weaponManager.allGuns)
        {
            if (toggled)
                gun.autoReload = true;
            else
                gun.autoReload = false;
        }
    }
    private void CoinCollected(int amount)
    {
        Debug.Log("a");
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

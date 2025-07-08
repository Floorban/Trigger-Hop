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
    [HideInInspector] public LevelEnd lvl;
    public PlayerController player;
    [HideInInspector] public WeaponManager weaponManager;
    public CameraController cam;
    public bool inLevel;

    [Header("UI")]
    public GameObject cancelAim;
    public RectTransform ammoUI;
    public Animator timerAnim;
    public int numOfCoin;
    public TextMeshProUGUI coinText, timerText;
    public Image coinResult, timerResult;
    public Sprite trueSprite, falseSprite;
    public TextMeshProUGUI timerRequirementText, coinRequirementText;
    public GameObject finalScreen, deathScreen, pauseScreen;
    public TextMeshProUGUI currentLevelText1, currentLevelText2;
    [SerializeField] private Toggle aimDir, autoReload;

    [Header("Global Time Control")]
    public float currentTime;
    public bool isPaused = false;
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

        currentTime -= Time.fixedDeltaTime;
        timerText.text = currentTime.ToString("0.0");
        if (currentTime <= 0)
        {
            currentTime = 0;
            LevelFinished(false);
        }
        else if (currentTime <= lvl.timeRequirement / 2f)
        {
            timerText.color = Color.red;
            timerAnim.speed = 2f;
        }
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
        finalScreen.SetActive(false);
        deathScreen.SetActive(false);
        pauseScreen.SetActive(false);
        cancelAim.SetActive(false);
        timerAnim.speed = 1f;
        timerAnim.SetBool("LevelEnd", false);
        timerRequirementText.color = Color.white;
        coinRequirementText.color = Color.white;
        weaponManager = p.GetComponentInChildren<WeaponManager>();
        audioManager.PlaySfx(audioManager.gameStart);
        inLevel = true;
        player = p;
        numOfCoin = 0;
        CoinCollected(0);
        currentTime = lvl.timeRequirement;
        isPaused = false;
        timerText.color = Color.white;
        coinText.color = Color.white;
        ToggleAimDir(aimDir.isOn);
        ToggleAutoReload(autoReload.isOn);
    }
    public void LevelFinished(bool win)
    {
        inLevel = false;
        isPaused = true;
        player.Stop();
        FindFirstObjectByType<WeaponManager>().StopAiming();
        currentLevelText1.text = "Level " + SceneManager.GetActiveScene().buildIndex;
        currentLevelText2.text = "Level " + SceneManager.GetActiveScene().buildIndex;
        timerRequirementText.text = "under " + lvl.timeRequirement / 2 + " seconds?";
        timerAnim.SetBool("LevelEnd", true);
        if (win)
        {
            audioManager.PlaySfx(audioManager.lvlFinished);
            CameraLock(lvl.lookAt, finalScreen);
            if (lvl.timeRequirement - currentTime <= lvl.timeRequirement / 2f)
                timerResult.sprite = trueSprite;
            else
                timerResult.sprite = falseSprite;
            if (numOfCoin == lvl.coinRequirement)
                coinResult.sprite = trueSprite;
            else
                coinResult.sprite = falseSprite;
        }
        else
        {
            Transform t = new GameObject("player lookAt", typeof(Transform)).transform;
            t.position = player.transform.position + new Vector3(-0.5f, -2f, 0);
            audioManager.PlaySfx(audioManager.gameOver);
            CameraLock(t, deathScreen);
        }
    }
    private void CameraLock(Transform target, GameObject screen)
    {
        var weaponManager = FindFirstObjectByType<WeaponManager>();

        weaponManager.inputLocked = true;
        cam.target = target;

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
             screen.SetActive(true);
             screen.transform.localScale = Vector3.one;
             screen.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 5, 0.5f);
             DOVirtual.DelayedCall(0.8f, () => ResultEffect());
             DOVirtual.DelayedCall(1.5f, () => TextEffects(lvl));
         });
    }
    private void ResultEffect()
    {
        coinResult.rectTransform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 5, 1f);
        timerResult.rectTransform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 5, 1f);
    }
    private void TextEffects(LevelEnd end)
    {
        if (end.timeRequirement - currentTime <= end.timeRequirement / 2f)
        {
            timerText.color = Color.green;
            timerRequirementText.color = Color.green;
            timerRequirementText.rectTransform.DOPunchScale(Vector3.one * 0.3f, 0.5f, 5, 1f);
        }
        else
        {
            timerText.color = Color.red;
            timerRequirementText.color = Color.red;
            timerRequirementText.rectTransform.DOShakePosition(0.3f, strength: new Vector3(5f, 0f, 0f));
            timerRequirementText.rectTransform.DOShakeRotation(0.4f, strength: 30f);
            cam.Shake();
        }

        if (numOfCoin == end.coinRequirement)
        {
            coinText.color = Color.green;
            coinRequirementText.color = Color.green;
            coinRequirementText.rectTransform.DOPunchScale(Vector3.one * 0.3f, 0.5f, 5, 1f);
        }
        else
        {
            coinText.color = Color.red;
            coinRequirementText.color = Color.red;
            coinRequirementText.rectTransform.DOShakeRotation(0.4f, strength: 30f);
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
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            LevelStarted(player);
        }
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
    public void Reload() => weaponManager.currentGun.Reload(weaponManager.currentGun.reloadDuration);
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

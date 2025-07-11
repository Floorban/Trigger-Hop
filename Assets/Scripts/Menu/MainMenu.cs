using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton, settingsButton, creditsButton, quitButton;
    [SerializeField] private GameObject mainMenu, levelSelectionMenu, settingsMenu, creditsMenu;
    [SerializeField] private AudioManager audioManager;
    private void Awake()
    {
        InitSubMenus();
        InitButtons();
    }
    private void InitSubMenus()
    {
        if (settingsMenu)
        {
            settingsMenu.SetActive(false);
            settingsMenu.transform.localScale = Vector3.zero; // Start scaled down for animation
        }
        if (creditsMenu)
        {
            creditsMenu.SetActive(false);
            creditsMenu.transform.localScale = Vector3.zero;
        }
        if (levelSelectionMenu)
        {
            levelSelectionMenu.SetActive(false);
            levelSelectionMenu.transform.localScale = Vector3.zero;
        }
        if (mainMenu) mainMenu.SetActive(true);
    }
    private void InitButtons()
    {
        RegisterButtonClick(startButton, StartGame);
        RegisterButtonClick(settingsButton, () => SubMenu(settingsButton, settingsMenu));
        RegisterButtonClick(quitButton, QuitGame);
        RegisterButtonClick(creditsButton, () => SubMenu(creditsButton, creditsMenu));
    }
    private void RegisterButtonClick(Button button, UnityAction onClickEvent)
    {
        if (!button || !button.interactable)
        {
            Debug.LogWarning($"{button.name} is not assigned in main menu!");
            return;
        }

        button.onClick.AddListener(onClickEvent);
        //button.onClick.AddListener(() => onClickEvent?.Invoke()); // using a wrapper for C# Action or UnityEvent
    }
    private void StartGame()
    {
        PlayButtonSfx(audioManager.sfxSource, audioManager.btnConfirm);
        PlayButtonAnim(startButton.gameObject, () =>
        {
            mainMenu.SetActive(false);
            // Logic after animation completes
            if (levelSelectionMenu)
            {
                levelSelectionMenu.SetActive(true);
                // Animate menu slide-in
                levelSelectionMenu.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }
        });
    }
    private void SubMenu(Button btn, GameObject menu)
    {
        PlayButtonSfx(audioManager.sfxSource, audioManager.btnSelect);
        PlayButtonAnim(btn.gameObject, () =>
        {
            mainMenu.SetActive(false);
            if (menu)
            {
                menu.SetActive(true);
                menu.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }
        });
    }
    private void QuitGame()
    {
        PlayButtonSfx(audioManager.sfxSource, audioManager.btnSelect);
/*        PlayButtonAnim(quitButton.gameObject, () =>
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });*/

        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void ReturnMainMenu(GameObject currentMenu)
    {
        PlayButtonSfx(audioManager.sfxSource, audioManager.btnSelect);
/*        PlayButtonAnim(backButton, () => 
            currentMenu.transform.DOScale(Vector3.zero, 0.8f)
                .SetEase(Ease.OutElastic, 1.5f)
                .OnComplete(() => currentMenu.SetActive(false)));*/

        currentMenu.SetActive(false);
        currentMenu.transform.localScale = Vector3.zero;

        mainMenu.SetActive(true);
        mainMenu.transform.localScale = Vector3.zero;
        mainMenu.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
    public void SocialButtonClicked(string website) => Application.OpenURL(website);
    private void PlayButtonSfx(AudioSource source, AudioClip clip)
    {
        if (source && clip) source.PlayOneShot(clip);
    }
    private void PlayButtonAnim(GameObject targetButton, UnityAction onComplete)
    {
        if (targetButton)
        {
            targetButton.transform.localScale = Vector3.one;
            targetButton.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 2, 0.2f) // Scale punch animation
                                   .SetEase(Ease.InOutQuad)
                                   .OnComplete(() => onComplete?.Invoke());
        }
        else
            onComplete?.Invoke();
    }
    private void OnDestroy()
    {
        if (startButton) startButton.onClick.RemoveAllListeners();
        if (settingsButton) settingsButton.onClick.RemoveAllListeners();
        if (quitButton) quitButton.onClick.RemoveAllListeners();
        DOTween.KillAll();
    }
}

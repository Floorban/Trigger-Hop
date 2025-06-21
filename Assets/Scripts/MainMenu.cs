using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using static Unity.VisualScripting.Member;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton, settingsButton,quitButton;
    [SerializeField] private GameObject levelSelectionMenu, settingsMenu;
    [SerializeField] private AudioSource buttonSound;
    [SerializeField] private AudioClip buttonClickClip;
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
        if (levelSelectionMenu)
        {
            levelSelectionMenu.SetActive(false);
            levelSelectionMenu.transform.localScale = Vector3.zero;
        }
    }
    private void InitButtons()
    {
        RegisterButtonClick(startButton, StartGame);
        RegisterButtonClick(settingsButton, SettingsMenu);
        RegisterButtonClick(quitButton, QuitGame);
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
        PlayButtonSfx(buttonSound, buttonClickClip);
        PlayButtonAnim(startButton.gameObject, () =>
        {
            // Logic after animation completes
            if (levelSelectionMenu)
            {
                levelSelectionMenu.SetActive(true);
                // Animate menu slide-in
                levelSelectionMenu.transform.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutElastic, 1.5f);
            }
        });
    }
    private void SettingsMenu()
    {
        PlayButtonSfx(buttonSound, buttonClickClip);
        PlayButtonAnim(settingsButton.gameObject, () =>
        {
            if (settingsMenu)
            {
                settingsMenu.SetActive(true);
                settingsMenu.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }
        });
    }
    private void QuitGame()
    {
        PlayButtonSfx(buttonSound, buttonClickClip);
        PlayButtonAnim(quitButton.gameObject, () =>
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });
    }
    private void PlayButtonSfx(AudioSource source, AudioClip clip)
    {
        if (source && clip) source.PlayOneShot(clip);
    }
    private void PlayButtonAnim(GameObject targetButton, UnityAction onComplete)
    {
        if (targetButton)
            targetButton.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 2, 0.2f).SetEase(Ease.InOutQuad).OnComplete(() => onComplete?.Invoke()); // Scale punch animation
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

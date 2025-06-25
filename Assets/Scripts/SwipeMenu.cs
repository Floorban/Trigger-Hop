using DG.Tweening;
using Lean.Touch;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class SwipeMenu : MonoBehaviour
{
    [SerializeField] int currentLevel;
    [SerializeField] int maxPage;
    int currentPage;
    Vector3 targetPos;
    [SerializeField] Vector3 pageStep;
    [SerializeField] RectTransform levelPagesRect;
    [SerializeField] float tweenTime;
    [SerializeField] Ease tweenType;
    private bool isSwiping;

    [SerializeField] LevelSelection[] levelSelections;
    [SerializeField] private List<Button> levelButtons;
    [SerializeField] Image[] pageImage;
    //[SerializeField] Sprite pageEnabled, pageDisabled;
    [SerializeField] Button nextButton, previousButton;

    private void Awake()
    {
        Debug.Log(PlayerPrefs.GetInt("UnlockedLevel", 1));
        currentPage = 1;
        levelPagesRect = GetComponent<RectTransform>();
        if (levelPagesRect) targetPos = levelPagesRect.localPosition;
        nextButton.onClick.AddListener(() => Next(nextButton));
        previousButton.onClick.AddListener(() => Previous(previousButton));
        UpdatePageBar();
        UpdateArrowButton();
    }
    private void HandleFingerSwipe(LeanFinger finger)
    {
        if (isSwiping || !finger.IsOverGui) return;

        isSwiping = true;
        Vector2 swipe = finger.SwipeScreenDelta;

        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        {
            if (swipe.x > 0.95f) Previous(previousButton);
            else Next(nextButton);
        }
        else
        {
            MovePage();
        }
    }
    public void Next(Button btn)
    {
        if (currentPage < maxPage)
        {
            ScaleEffect(btn.GetComponent<RectTransform>(), 1.3f, 1f);
            currentPage++;
            targetPos += pageStep;
            MovePage();
        }
    }
    public void Previous(Button btn)
    {
        if (currentPage > 1)
        {
            ScaleEffect(btn.GetComponent<RectTransform>(), 1.3f, 1f);
            currentPage--;
            targetPos -= pageStep;
            MovePage();
        }
    }
    private void MovePage()
    {
        if (levelPagesRect != null)
        {
            UpdatePageBar();
            UpdateArrowButton();
            levelPagesRect.DOLocalMove(targetPos, tweenTime)
                .SetEase(tweenType)
                .OnComplete(() => FinishSwipe());
        }
    }
    private void UpdatePageBar()
    {
        foreach(var page in pageImage)
        {
            //page.sprite = pageDisabled;
            page.color = Color.gray;
            RectTransform buttonRect = page.GetComponent<RectTransform>();
            buttonRect.DOKill();
            buttonRect.DOScale(1f, 0.1f).SetEase(Ease.InQuad);
        }
        //pageImage[currentPage - 1].sprite = pageEnabled;
        pageImage[currentPage - 1].color = Color.green;

        foreach(var level in levelSelections)
        {
            level.isSnapping = true;
            level.canSelect = false;
        }

        levelSelections[currentPage - 1].canSelect = true;
    }
    private void FinishSwipe()
    {
        isSwiping = false;
        ScaleEffect(pageImage[currentPage - 1].GetComponent<RectTransform>(), 1.6f, 1.3f);
        foreach (var level in levelSelections)
        {
            level.isSnapping = true;
        }
        levelSelections[currentPage - 1].InitFirstLevelButton(0);
    }
    private void UpdateArrowButton()
    {
        nextButton.interactable = true;
        previousButton.interactable = true;
        if (currentPage == 1) previousButton.interactable = false;
        else if (currentPage == maxPage) nextButton.interactable = false;
    }
    private void ScaleEffect(RectTransform target, float zoomScale, float endScale)
    {
        target.DOScale(zoomScale, 0.15f).SetEase(Ease.OutQuad)
            .OnComplete(() => target.DOScale(endScale, 0.1f).SetEase(Ease.InQuad));
    }
    public void GetSelectedLevel(int lvlIndex)
    {
        currentLevel = lvlIndex;
    }
    public void StartLevel()
    {
        if (currentLevel <= 0 || !levelSelections[currentPage - 1].canSelect || currentLevel > PlayerPrefs.GetInt("UnlockedLevel", 1)) return;

        Debug.Log($"Loading Level {currentLevel}");
        SceneManager.LoadScene(currentLevel);
    }
    private void UnlockLevel()
    {
        int unlockedLvl = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < levelButtons.Count; i++)
        {
            //levelButtons[i].interactable = false;
        }
        for (int i = 0; i < unlockedLvl; i++)
        {
            levelButtons[i].interactable = true;
        }
    }
    private void GetLevelButtons()
    {
        for (int i = 0; i < levelSelections.Length; i++)
        {
            for (int j = 0; j < levelSelections[i].levelButtons.Length; j++)
            {
                levelButtons.Add(levelSelections[i].levelButtons[j]);
            }
        }
    }
    public void ClearLevelData()
    {
        PlayerPrefs.DeleteAll();
    }
    private void OnEnable()
    {
        LeanTouch.OnFingerSwipe += HandleFingerSwipe;
        LevelSelection.OnLevelSelected += GetSelectedLevel;
        GetLevelButtons();
        UnlockLevel();
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerSwipe -= HandleFingerSwipe;
        LevelSelection.OnLevelSelected -= GetSelectedLevel;
    }

}
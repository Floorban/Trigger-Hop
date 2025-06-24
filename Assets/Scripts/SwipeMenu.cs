using DG.Tweening;
using Lean.Touch;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] Image[] pageImage;
    //[SerializeField] Sprite pageEnabled, pageDisabled;

    private void Awake()
    {
        currentPage = 1;
        levelPagesRect = GetComponent<RectTransform>();
        if (levelPagesRect) targetPos = levelPagesRect.localPosition;
        UpdatePageBar();
    }
    private void HandleFingerSwipe(LeanFinger finger)
    {
        if (isSwiping || !finger.IsOverGui) return;

        isSwiping = true;
        Vector2 swipe = finger.SwipeScreenDelta;

        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        {
            if (swipe.x > 0.9f) Previous();
            else Next();
        }
        else
        {
            MovePage();
        }
    }

    public void Next()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
            targetPos += pageStep;
            MovePage();
        }
    }
    public void Previous()
    {
        if (currentPage > 1)
        {
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

            levelPagesRect.DOLocalMove(targetPos, tweenTime)
                .SetEase(tweenType)
                .OnComplete(() =>
                {
                    isSwiping = false;
                    RectTransform image = pageImage[currentPage - 1].GetComponent<RectTransform>();
                    image.DOScale(1.6f, 0.15f).SetEase(Ease.OutQuad)
                        .OnComplete(() => image.DOScale(1.3f, 0.1f).SetEase(Ease.InQuad));
                    foreach (var level in levelSelections)
                    {
                        level.isSnapping = true;
                    }
                });
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

        levelSelections[currentPage - 1].InitFirstLevelButton(0);
        levelSelections[currentPage - 1].canSelect = true;
    }
    public void GetSelectedLevel(int lvlIndex)
    {
        currentLevel = lvlIndex;
    }
    public void StartLevel()
    {
        for (int i = 0; i < levelSelections.Length; i++)
        {
            if (!levelSelections[i].canSelect) return;
        }

        if (currentLevel <= 0) return;
        Debug.Log($"Loading Level {currentLevel}");
        SceneManager.LoadScene(currentLevel);
    }
    private void OnEnable()
    {
        LeanTouch.OnFingerSwipe += HandleFingerSwipe;
        LevelSelection.OnLevelSelected += GetSelectedLevel;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerSwipe -= HandleFingerSwipe;
        LevelSelection.OnLevelSelected -= GetSelectedLevel;
    }

}
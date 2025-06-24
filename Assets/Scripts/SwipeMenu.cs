using DG.Tweening;
using Lean.Touch;
using UnityEngine;
using UnityEngine.UI;

public class SwipeMenu : MonoBehaviour
{
    [SerializeField] int maxPage;
    int currentPage;
    Vector3 targetPos;
    [SerializeField] Vector3 pageStep;
    [SerializeField] RectTransform levelPagesRect;
    [SerializeField] float tweenTime;
    [SerializeField] Ease tweenType;
    private bool isSwiping;

    private void Awake()
    {
        currentPage = 1;
        levelPagesRect = GetComponent<RectTransform>();
        if (levelPagesRect) targetPos = levelPagesRect.localPosition;
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
            levelPagesRect.DOLocalMove(targetPos, tweenTime)
                .SetEase(tweenType)
                .OnComplete(() => isSwiping = false);
        }
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerSwipe += HandleFingerSwipe;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerSwipe -= HandleFingerSwipe;
    }

}
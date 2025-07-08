using DG.Tweening;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;
public enum GestureType
{
    Drag,
    Swipe,
    Pinch
}
public class GestureTutorial : MonoBehaviour
{
    [SerializeField] private RectTransform fingerTransform;
    private Image fingerImage;
    public Sprite drag, swipe, pinch;
    public float animationDuration = 1f;
    private void Awake()
    {
        fingerTransform = GetComponent<RectTransform>();
        fingerImage = GetComponent<Image>();
    }
    public void Show(GestureType gesture)
    {
        fingerTransform.gameObject.SetActive(true);

        switch (gesture)
        {
            case GestureType.Drag:
                fingerImage.sprite = drag;
                PlayDrag();
                break;
            case GestureType.Swipe:
                fingerImage.sprite = swipe;
                PlaySwipe();
                break;
            case GestureType.Pinch:
                fingerImage.sprite = pinch;
                PlayPinch();
                break;
        }
    }

    public void Hide()
    {
        fingerTransform.DOKill();
        fingerTransform.gameObject.SetActive(false);
    }

    private void PlayDrag()
    {
        Vector2 start = new Vector2(-200f, 0f);
        Vector2 end = new Vector2(200f, 0f);
        fingerTransform.anchoredPosition = start;

        fingerTransform.DOAnchorPos(end, animationDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);
    }

    private void PlaySwipe()
    {
        Vector2 start = new Vector2(0f, 150f);
        Vector2 end = new Vector2(0f, -150f);
        fingerTransform.anchoredPosition = start;

        fingerTransform.DOAnchorPos(end, animationDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);
    }

    private void PlayPinch()
    {
        fingerTransform.anchoredPosition = new Vector2(0f, 350f);
        fingerTransform.localScale = Vector3.one;

        fingerTransform.DOScale(0.5f, animationDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);
    }
}

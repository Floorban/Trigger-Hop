using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
public enum GestureType
{
    Drag,
    Swipe,
    Pinch,
    Shake
}
public class GestureTutorial : MonoBehaviour
{
    [SerializeField] private RectTransform fingerTransform;
    private Image fingerImage;
    public Sprite drag, swipe, pinch, shake;
    public TextMeshProUGUI prompt;
    public float animationDuration = 1f;
    private void Awake()
    {
        fingerTransform = GetComponent<RectTransform>();
        fingerImage = GetComponent<Image>();
    }
    public void Show(GestureType gesture)
    {
        fingerTransform.DOKill();
        fingerTransform.gameObject.SetActive(true);
        prompt.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -600f);
        prompt.gameObject.SetActive(true);

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
            case GestureType.Shake:
                fingerImage.sprite = shake;
                PlayShake();
                break;
        }
    }

    public void Hide()
    {
        fingerTransform.DOKill();
        fingerTransform.gameObject.SetActive(false);
        prompt.gameObject.SetActive(false);
    }

    private void PlayDrag()
    {
        fingerTransform.anchoredPosition = new Vector2(-150f, 150f);
        prompt.text = "drag to shoot";

        fingerTransform.DOAnchorPos(new Vector2(150f, 50f), animationDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);

        fingerTransform.DOAnchorPosY(20f, animationDuration / 2f)
            .SetRelative()
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);
    }

    private void PlaySwipe()
    {
        Vector2 start = new Vector2(0f, 300f);
        Vector2 end = new Vector2(0f, 0f);
        fingerTransform.anchoredPosition = start;
        prompt.text = "swipe to switch weapon";

        fingerTransform.DOAnchorPos(end, animationDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);
    }

    private void PlayPinch()
    {
        fingerTransform.anchoredPosition = new Vector2(0f, 350f);
        fingerTransform.localScale = Vector3.one;
        prompt.text = "";

        fingerTransform.DOScale(0.5f, animationDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);
    }
    private void PlayShake()
    {
        fingerTransform.anchoredPosition = new Vector2(-10f, 200f);
        fingerTransform.localScale = Vector3.one;
        prompt.text = "shake / tap corner gun icon to reload\r\n";

        fingerTransform.DOShakeAnchorPos(
            duration: animationDuration,
            strength: new Vector2(20f, 20f),
            vibrato: 10,
            randomness: 90,
            snapping: false,
            fadeOut: true)
        .SetLoops(-1, LoopType.Restart)
        .SetUpdate(true);
    }
}

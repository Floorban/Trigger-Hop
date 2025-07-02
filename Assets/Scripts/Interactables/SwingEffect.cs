using DG.Tweening;
using UnityEngine;

public class SwingEffect : MonoBehaviour
{
    private RectTransform uiElement;
    public float swingAngle = 15f;
    public float swingDuration = 2;

    void Start()
    {
        uiElement = GetComponent<RectTransform>();
        StartSwing();
    }

    void StartSwing()
    {
        uiElement.rotation = Quaternion.identity;
        Sequence swingSeq = DOTween.Sequence();

        bool startRight = Random.value > 0.5f; // Randomly choose left or right start

        if (startRight)
        {
            swingSeq.Append(uiElement.DORotate(new Vector3(0, 0, swingAngle), swingDuration).SetEase(Ease.InOutSine));
            swingSeq.Append(uiElement.DORotate(new Vector3(0, 0, -swingAngle), swingDuration).SetEase(Ease.InOutSine));
            swingSeq.Append(uiElement.DORotate(new Vector3(0, 0, swingAngle), swingDuration).SetEase(Ease.InOutSine));
            swingSeq.Append(uiElement.DORotate(Vector3.zero, swingDuration).SetEase(Ease.InOutSine));
        }
        else
        {
            swingSeq.Append(uiElement.DORotate(new Vector3(0, 0, -swingAngle), swingDuration).SetEase(Ease.InOutSine));
            swingSeq.Append(uiElement.DORotate(new Vector3(0, 0, swingAngle), swingDuration).SetEase(Ease.InOutSine));
            swingSeq.Append(uiElement.DORotate(new Vector3(0, 0, -swingAngle), swingDuration).SetEase(Ease.InOutSine));
            swingSeq.Append(uiElement.DORotate(Vector3.zero, swingDuration).SetEase(Ease.InOutSine));
        }

        swingSeq.SetLoops(-1);
    }
}

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Lean.Touch;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private float radius = 200f;  // Distance from center to buttons 
    [SerializeField] private float friction = 0.98f; 
    [SerializeField] private float rotationSensitivity = 0.5f;
    [SerializeField] private bool canSelect = true;
    [SerializeField] private AudioSource buttonSound;
    [SerializeField] private AudioClip buttonClickClip;

    private float currentAngle;
    private float angleStep;
    private float angularVelocity;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        SetupRadialLayout();
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerUpdate += HandleFingerUpdate;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
    }
    private bool isSnapping = false;

    private void Update()
    {
        if (Mathf.Abs(angularVelocity) > 0.01f)
        {
            currentAngle += angularVelocity * Time.deltaTime;
            rectTransform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            angularVelocity *= friction;

            // Once it's slow enough, prepare to snap, since the vel is reduced under the > 0.01 condition it can still run this logics inside
            if (Mathf.Abs(angularVelocity) <= 0.01f && !isSnapping)
            {
                angularVelocity = 0f;
                isSnapping = true;
                SnapToNearestSlot();
            }
        }
        else if (!isSnapping)
        {
            // Ensure don't get stuck in a weird idle state
            angularVelocity = 0f;
        }

        UpdateButtonRotations();
    }
    private void SnapToNearestSlot()
    {
        if (levelButtons == null || levelButtons.Length == 0) return;

        float normalizedAngle = Mathf.Repeat(currentAngle, 360f);
        float targetAngle = Mathf.Round(normalizedAngle / angleStep) * angleStep;

        float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);

        DOTween.To(() => currentAngle, x => currentAngle = x, currentAngle + angleDiff, 0.6f)
            .SetEase(Ease.OutExpo)
            .OnUpdate(() => rectTransform.rotation = Quaternion.Euler(0f, 0f, currentAngle))
            .OnComplete(() => isSnapping = false);
    }

    private void SetupRadialLayout()
    {
        if (levelButtons == null || levelButtons.Length == 0) return;

        angleStep = 360f / levelButtons.Length;
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                // Start button 0 at top with no rotation
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector2 pos = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
                RectTransform buttonRect = levelButtons[i].GetComponent<RectTransform>();
                buttonRect.anchoredPosition = pos;
                int index = i;
                levelButtons[i].onClick.AddListener(() => SelectLevel(index, -i * angleStep));
            }
        }
    }

    private void UpdateButtonRotations()
    {
        if (levelButtons == null || levelButtons.Length == 0) return;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                float angle = -i * angleStep + currentAngle;
                RectTransform buttonRect = levelButtons[i].GetComponent<RectTransform>();
                buttonRect.rotation = Quaternion.Euler(0f, 0f, angle); // Rotate outward
            }
        }
    }

    private void HandleFingerUpdate(LeanFinger finger)
    {
        if (finger.IsOverGui)
        {
            float deltaAngle = -finger.ScaledDelta.x * rotationSensitivity;
            currentAngle += deltaAngle;
            rectTransform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            angularVelocity = deltaAngle / Time.deltaTime * 0.5f;
        }
    }

    private void SelectLevel(int index, float buttonAngle)
    {
        if (!canSelect) return;

        float normalizedAngle = Mathf.Repeat(currentAngle, 360f);
        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(normalizedAngle, buttonAngle));
        if (angleDiff < 15f)
        {
            PlayButtonSfx();
            Debug.Log($"Loading Level {index + 1}");
            // TODO: Load level (e.g., SceneManager.LoadScene(index + 1));
        }
    }

    private void PlayButtonSfx()
    {
        if (buttonSound != null && buttonClickClip != null)
            buttonSound.PlayOneShot(buttonClickClip);
    }

    private void OnDestroy()
    {
        foreach (var button in levelButtons)
            if (button != null) button.onClick.RemoveAllListeners();
        DOTween.Kill(rectTransform);
        LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
    }
}

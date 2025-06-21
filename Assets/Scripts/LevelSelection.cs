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

    private void Update()
    {
        if (Mathf.Abs(angularVelocity) > 0.01f)
        {
            currentAngle += angularVelocity * Time.deltaTime;
            rectTransform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            angularVelocity *= friction;
        }
        UpdateButtonRotations();
    }

    private void SetupRadialLayout()
    {
        if (levelButtons == null || levelButtons.Length == 0) return;

        float angleStep = 360f / levelButtons.Length;
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                // Start button 0 at top (0бу)
                float angle = -i * angleStep * Mathf.Deg2Rad; // Negative to put 0 at top
                Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
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

        float angleStep = 360f / levelButtons.Length;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                float angle = -i * angleStep + currentAngle; // Apply rotation
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
        {
            buttonSound.PlayOneShot(buttonClickClip);
            Debug.Log("Playing button sound...");
        }
    }

    private void OnDestroy()
    {
        foreach (var button in levelButtons)
            if (button != null) button.onClick.RemoveAllListeners();
        DOTween.Kill(rectTransform);
        LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
    }
}

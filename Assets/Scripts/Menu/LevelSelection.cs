using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Lean.Touch;
using UnityEngine.Events;

public class LevelSelection : MonoBehaviour
{
    public Button[] levelButtons;
    private int currentLevel;
    private int lastTopButtonIndex = -1;
    [SerializeField] private float radius = 200f;
    [SerializeField] private float friction = 0.98f;
    [SerializeField] private float rotationSensitivity = 0.5f;
    public bool canSelect;

    private float currentAngle;
    private float angleStep;
    private float angularVelocity;
    private RectTransform rectTransform;
    [HideInInspector] public bool isSnapping;

    public static UnityAction<int> OnLevelSelected;
    public int previousLvls;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        rectTransform = GetComponent<RectTransform>();
        SetupRadialLayout();
    }
    private void Start()
    {
        InitFirstLevelButton(0);
    }
    private void SetupRadialLayout()
    {
        currentAngle = 0f;
        rectTransform.rotation = Quaternion.Euler(0f, 0f, currentAngle);

        if (!ValidateButtons()) return;

        angleStep = 360f / levelButtons.Length;
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector2 pos = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
                RectTransform buttonRect = levelButtons[i].GetComponent<RectTransform>();
                buttonRect.anchoredPosition = pos;
                int index = i;
                levelButtons[i].onClick.AddListener(() => SnapSelectedButtonToTop(index));
            }
        }
    }
    public void InitFirstLevelButton(int buttonIndex)
    {
        if (!ValidateButtons()) return;
        SelectLevel(buttonIndex);
        SnapSelectedButtonToTop(buttonIndex);
    }
    private void Update()
    {
        if (Mathf.Abs(angularVelocity) > 0.01f)
        {
            currentAngle += angularVelocity * Time.deltaTime;
            rectTransform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            angularVelocity *= friction;

            if (Mathf.Abs(angularVelocity) <= 5f)
            {
                if (!isSnapping) SnapSelectedButtonToTop(GetNearestButtonIndex());
                angularVelocity = 0f;
                isSnapping = true;
            }
        }
        else if (!isSnapping)
        {
            angularVelocity = 0f;
        }

        UpdateButtonRotations();
    }
    private void HandleFingerUpdate(LeanFinger finger)
    {
        if (!finger.IsOverGui) return;

        Vector2 screenCenter = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);

        Vector2 prev = finger.LastScreenPosition;
        Vector2 curr = finger.ScreenPosition;

        Vector2 prevDir = (prev - screenCenter).normalized;
        Vector2 currDir = (curr - screenCenter).normalized;

        float angleDelta = Vector2.SignedAngle(prevDir, currDir);

        currentAngle += angleDelta;
        rectTransform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
        angularVelocity = angleDelta * rotationSensitivity / Time.deltaTime;
    }
    private void HandleFingerUp(LeanFinger finger)
    {
        angularVelocity = 0.1f;

        /*        if (finger.IsOverGui)  //&& !isSnapping
                    angularVelocity = 0.1f;*/
    }
    private void UpdateButtonRotations()
    {
        if (!ValidateButtons()) return;

        int topButtonIndex = GetNearestButtonIndex();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                float angle = -i * angleStep + currentAngle;
                RectTransform buttonRect = levelButtons[i].GetComponent<RectTransform>();
                buttonRect.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        if (topButtonIndex != lastTopButtonIndex) //&& !isSnapping
        {
            ScaleDownNonSelectedButtons();
            lastTopButtonIndex = topButtonIndex;
        }
    }
    private void SelectLevel(int index)
    {
        if (!canSelect) return;
        audioManager.PlaySfx(audioManager.spin);
        currentLevel = index + 1 + previousLvls;
        //RotateButtonToTop(index);
        Debug.Log($"Selected Level {currentLevel}");
        OnLevelSelected?.Invoke(currentLevel);
    }

/*    public void StartLevel()
    {
        if (!canSelect || currentLevel <= 0) return;

        Debug.Log($"Loading Level {currentLevel}");
        PlayButtonSfx();
        SceneManager.LoadScene(currentLevel);
    }*/

    public void SnapSelectedButtonToTop(int buttonIndex)
    {
        if (!ValidateButtons()) return;

        float targetButtonAngle = buttonIndex * angleStep;
        float normalizedAngle = Mathf.Repeat(currentAngle, 360f);
        float angleDiff = Mathf.DeltaAngle(normalizedAngle, targetButtonAngle);

        isSnapping = true;

        DOTween.To(() => currentAngle, x => currentAngle = x, currentAngle + angleDiff, 0.2f)
            .SetEase(Ease.InOutCubic)
            .OnUpdate(() => rectTransform.rotation = Quaternion.Euler(0f, 0f, currentAngle))
            .OnComplete(() =>
            {
                isSnapping = false;
                PlayScaleEffect(buttonIndex);
                //ScaleDownNonSelectedButtons();
                lastTopButtonIndex = buttonIndex;
                SelectLevel(lastTopButtonIndex);
            });

    }

    private void PlayScaleEffect(int buttonIndex)
    {
        if (!ValidateButtons()) return;

        RectTransform btn = levelButtons[buttonIndex].GetComponent<RectTransform>();
        btn.DOKill();
        btn.localScale = Vector3.one;

/*        Sequence scaleSequence = DOTween.Sequence();
        scaleSequence.Append(btn.DOScale(1.5f, 0.15f).SetEase(Ease.OutQuad));
        scaleSequence.Append(btn.DOScale(1f, 0.1f).SetEase(Ease.InQuad));*/

        btn.DOScale(1.7f, 0.2f).SetEase(Ease.OutQuad)
            .OnComplete(() => btn.DOScale(1.4f, 0.15f).SetEase(Ease.InQuad));
    }

    private void ScaleDownNonSelectedButtons()
    {
        if (!ValidateButtons()) return;

        int topButtonIndex = GetNearestButtonIndex();
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null && i != topButtonIndex)
            {
                RectTransform buttonRect = levelButtons[i].GetComponent<RectTransform>();
                buttonRect.DOKill();
                buttonRect.DOScale(1f, 0.15f).SetEase(Ease.InQuad);
            }
        }
    }

    private bool ValidateButtons()
    {
        return levelButtons != null && levelButtons.Length > 0;
    }

    private int GetNearestButtonIndex()
    {
        float normalizedAngle = Mathf.Repeat(currentAngle, 360f);
        return Mathf.RoundToInt(normalizedAngle / angleStep) % levelButtons.Length;
    }
    private void OnEnable()
    {
        LeanTouch.OnFingerUpdate += HandleFingerUpdate;
        LeanTouch.OnFingerUp += HandleFingerUp;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
        LeanTouch.OnFingerUp -= HandleFingerUp;
    }

    private void OnDestroy()
    {
        if (levelButtons != null)
        {
            foreach (var button in levelButtons)
                if (button != null) button.onClick.RemoveAllListeners();
        }
        DOTween.Kill(rectTransform);
        LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
    }
}
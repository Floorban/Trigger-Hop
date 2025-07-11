using UnityEngine;
using System.Collections.Generic;
using Solo.MOST_IN_ONE;

public class WeaponManager : MonoBehaviour
{
    public bool inputLocked;
    [HideInInspector] public PlayerController player;
    private IInput input;

    public List<GunController> allGuns;
    public GunController currentGun;
    private int currentIndex = 0;

    [Header("Aiming")]
    [HideInInspector] public int inputAimDir = 1;
    private LineRenderer aimLine;
    public bool isAiming = false;
    [SerializeField] private float slowMotionScale = 0.3f;

    [Header("Switch Weapon")]
    [SerializeField] private float cycleCooldown = 0.3f;
    private float lastCycleTime = -Mathf.Infinity;
    public Vector2 swipeStartPos;
    public bool isSwipingTwoFingers = false;

    [Header("Raycast Settings")]
    public int reflections;
    public float maxRayDistance = 10f;
    public LayerMask layerDetection;

    [Header("InputIntensity")]
    public float shakeIntensity = 10f;
    public float cycleDistance = 100f;

    private void Start() {
        player = GetComponentInParent<PlayerController>();
        Physics2D.queriesStartInColliders = false;
        aimLine = GetComponent<LineRenderer>();
        if (aimLine) aimLine.enabled = false;
        if (allGuns.Count > 0) currentGun = allGuns[0];
        foreach (var g in allGuns)
        {
            g.gameObject.SetActive(false);
        }
        if (currentGun)
            currentGun.Setup(player);

#if UNITY_EDITOR || UNITY_STANDALONE
        input = new MobileInput();
        //input = new DesktopInput();
#elif UNITY_IOS || UNITY_ANDROID
        input = new MobileInput();
#endif
    }
    private void Update() {
        // organize desktop and mobile input with seperate scripts (MobileInput and DesktopInput) now (using IInput Interface)
        if (inputLocked) return;
        input?.HandleInput(this);
        //HandleShootInput();
        //HandleReloadInput();
        //HandleSwitchWeaponInput();
    }

    public void CycleGun(float cycleOrder) {
        if (Time.time - lastCycleTime <= cycleCooldown || allGuns.Count <= 1) return;

        if (cycleOrder < 0) { // cycle down
            currentIndex = (currentIndex + 1) % allGuns.Count; 
        }
        else { // cycle up
            if (currentIndex > 0)
                currentIndex = (currentIndex - 1) % allGuns.Count;
            else
                currentIndex = allGuns.Count - 1;
        }
        currentGun.ClearAmmoUI();
        currentGun.gameObject.SetActive(false);
        currentGun = allGuns[currentIndex];
        if (currentGun)
            currentGun.Setup(player);
        if (!SceneController.instance.hasSwipped)
        {
            SceneController.instance.hasSwipped = true;
            SceneController.instance.gesture.Hide();
        }
        lastCycleTime = Time.time;
    }
    public void StartAiming() {
        if (currentGun.fireMode == GunBase.FireMode.Auto || isAiming) return;
        if (!SceneController.instance.hasDragged)
        {
            SceneController.instance.hasDragged = true;
            SceneController.instance.gesture.Hide();
        }
        isAiming = true;
        SceneController.instance.cancelAim.SetActive(true);
        SceneController.instance.SetScaledTime(slowMotionScale);
        if (aimLine) aimLine.enabled = true;
    }
    public void StopAiming() {
        isAiming = false;
        SceneController.instance.cancelAim?.SetActive(false);
        SceneController.instance.SetScaledTime(1f);
        if (aimLine) aimLine.enabled = false;
    }
    public void UpdateAimLine(Vector2 touchWorldPos) {
        if (!aimLine) return;
        player.transform.right = touchWorldPos.normalized;
        aimLine.positionCount = 2;
        aimLine.SetPosition(0, transform.position);
        aimLine.SetPosition(1, (Vector2)transform.position + touchWorldPos * inputAimDir);

        //aimLine.SetPosition(1, touchWorldPos.normalized * maxRayDistance);
        /*RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, currentGun.aimDir, maxRayDistance, layerDetection);
        if (hitInfo)
            aimLine.SetPosition(1, hitInfo.point);*/
    }

    /// <summary>
    /// old code, now coverted all the logic to MobileInput and DesktopInput
    /// </summary>
    /// 
/*    private void HandleShootInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        if (Input.GetMouseButtonDown(0))
        {
            currentGun.OnTouchBegin(Input.mousePosition);
            StartAiming();
        }
        else if (Input.GetMouseButton(0))
        {
            currentGun.OnTouchDrag(Input.mousePosition);
            UpdateAimLine(currentGun.aimDir);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            currentGun.OnTouchEnd();
            StopAiming();
        }
#elif UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount == 1) {
            Touch touch = Input.GetTouch(0);
            if (isSwipingTwoFingers) return;
            switch (touch.phase) {
                case TouchPhase.Began:
                    currentGun.OnTouchBegin(touch.position);
                    StartAiming();
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    currentGun.OnTouchDrag(touch.position);
                    UpdateAimLine(currentGun.aimDir);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    currentGun.OnTouchEnd();
                    StopAiming();
                    break;
            }
        }
        else {
            StopAiming();
        }
#endif
    }
    private void HandleReloadInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        if (Input.GetKeyDown(KeyCode.R))
            currentGun.Reload();
#elif UNITY_IOS || UNITY_ANDROID
        Vector3 acc = Input.acceleration;
        if (acc.sqrMagnitude > shakeIntensity) {
            currentGun.Reload();
        }
#endif
    }

    private void HandleSwitchWeaponInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        if (Input.mouseScrollDelta.y != 0)
            CycleGun(Input.mouseScrollDelta.y);
#elif UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount == 2) {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            if (t1.phase == TouchPhase.Began || t2.phase == TouchPhase.Began) {
                swipeStartPos = (t1.position + t2.position) / 2f;
                isSwipingTwoFingers = true;
            }
            else if ((t1.phase == TouchPhase.Ended || t1.phase == TouchPhase.Canceled) &&
                          (t2.phase == TouchPhase.Ended || t2.phase == TouchPhase.Canceled) &&
                          isSwipingTwoFingers) {
                Vector2 swipeEndPos = (t1.position + t2.position) / 2f;
                Vector2 swipeDelta = swipeEndPos - swipeStartPos;

                if (Mathf.Abs(swipeDelta.y) > cycleDistance) {
                        CycleGun(swipeDelta.y);
                }

                isSwipingTwoFingers = false;
            }
        }
        else {
            isSwipingTwoFingers = false;  
        }
#endif
    }*/
}

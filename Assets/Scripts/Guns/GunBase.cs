using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Solo.MOST_IN_ONE;

public class GunBase : MonoBehaviour {
    public enum FireMode {
        Manual,
        Auto,
        Charge
    }
    protected PlayerController player;

    [Header("Gun Stats")]
    public bool reverseAimDir;
    [HideInInspector] public int  inputAimDIr = 1;
    public bool autoReload = true;
    public int clipSize = 6;
    public float cooldown = 0.3f;
    public float reloadDuration = 1f;
    public float bulletSpeed = 20f;
    public float recoilForce = 5f;
    [SerializeField] protected float bulletLifetime = 1f;
    public int baseDamage;

    [Header("UI")]
    public RectTransform uiCanvas;
    public Sprite gunUI;
    [SerializeField] protected GameObject bulletUI;
    protected List<GameObject> bulletUIList = new List<GameObject>();

    [Header("References")]
    protected Animator animator;
    [SerializeField] protected GameObject bulletPrefab;
    public Transform shootPoint;
    public Animator muzzle;
    public AudioClip fireSfx;
    public AudioClip bulletSfx;
    public AudioClip reloadSfx;
    public Most_HapticFeedback.CustomHapticPattern hapticPattern;

    protected int currentAmmo;
    public bool canShoot = false;
    protected float lastShotTime;
    protected bool isReloading = false;
    [HideInInspector] public  Vector2 aimDir;
    private void Awake() {
        if (isPickup) GetComponent<Collider2D>().isTrigger = true;
        animator = GetComponent<Animator>();
        uiCanvas = GameObject.Find("Ammo").GetComponent<RectTransform>();
        currentAmmo = clipSize;
        inputAimDIr = GetInputDir(reverseAimDir);
        StartSwing();
    }
    public virtual void Setup(PlayerController p) {
        player = p;
        inputAimDIr = GetInputDir(reverseAimDir);
        lastShotTime = -1;
        gameObject.SetActive(true);
        ClearAmmoUI();
        Debug.Log(name + " in hand");
        Reload(0f);
    }

    /*    // handle different fire logic with abstract class now
     *    protected void Fire(Vector2 direction) {
            if (!canShoot || currentAmmo <= 0)
                return;

            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = direction.normalized * bulletSpeed;

            playerRb.AddForce(-direction.normalized * recoilForce, ForceMode2D.Impulse);
            currentAmmo--;
            canShoot = false;
            Invoke(nameof(ResetCooldown), cooldown);
        }*/
    protected Vector2 GetAimDir(Vector2 screenPos) {
        Vector2 worldPos = Camera.main.ScreenToViewportPoint(screenPos);
        return (worldPos - (Vector2)transform.position);
    }
    private int GetInputDir(bool reverseInputDir) {
        if (reverseInputDir)
            return -1;
        else
            return 1;
    }
    public bool TryFire(int consumedAmmo, FireMode fireMode) {
        // try consume ammo and check cooldown first
        if (currentAmmo <= 0 || (fireMode != FireMode.Auto && !canShoot) || Time.time - lastShotTime < cooldown)
            return false;

        // - used amount of ammo
        if (fireMode != FireMode.Charge) {
            currentAmmo -= consumedAmmo;
            UpdateAmmoUI(consumedAmmo);
            SceneController.instance.cam.Shake();
            muzzle.SetTrigger("Fire");
            StartCoroutine(Most_HapticFeedback.GeneratePattern(hapticPattern));
            SceneController.instance.audioManager.PlaySfx(fireSfx);
            Debug.Log(currentAmmo + " left");
        }

        if (fireMode != FireMode.Auto) {
            canShoot = false;
            Invoke(nameof(ResetCooldown), cooldown);
        }

        if (currentAmmo <= 0) {
            canShoot = false;
            if (autoReload) {
                Reload(reloadDuration);
            }
        }

        // activate cooldown
        lastShotTime = Time.time;
        return true;
    }
    protected void ResetCooldown() {
        canShoot = true;
    }
    public void Reload(float reloadTime) {
        if (isReloading) return;
        isReloading = true;
        Debug.Log("reloading");
        Invoke(nameof(FinishReload), reloadTime);
    }
    protected void FinishReload() {
        currentAmmo = clipSize;
        isReloading = false;
        ReloadAmmoUI();
        canShoot = true;
        animator.SetTrigger("Reload");
        StartCoroutine(Most_HapticFeedback.GeneratePattern(hapticPattern));
        SceneController.instance.audioManager.PlaySfx(reloadSfx);
        Debug.Log("finish reload");
    }
    protected void UpdateAmmoUI(int removedNum)
    {
        if (bulletUIList == null || bulletUIList.Count == 0) return;

        int indexToRemove = currentAmmo;
        if (indexToRemove >= 0 && indexToRemove < bulletUIList.Count)
        {
            for (int i = currentAmmo + removedNum - 1; i >= currentAmmo; i--)
            {
                GameObject bullet = bulletUIList[i];
                bullet.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack)
                    .OnComplete(() => bullet.SetActive(false));
            }
        }
    }
    protected void ReloadAmmoUI()
    {
        if (!uiCanvas || !bulletUI) return;

        foreach (var b in bulletUIList)
        {
            Destroy(b);
        }
        bulletUIList.Clear();

        for (int i = 0; i < clipSize; i++)
        {
            GameObject bullet = Instantiate(bulletUI);
            bullet.transform.SetParent(uiCanvas, false);

            RectTransform rect = bullet.GetComponent<RectTransform>();
            float xOffset = Random.Range(-5f, 5f);
            float yOffset = Random.Range(-8f, 8f);
            float randomZRotation = Random.Range(-10f, 10f);

            rect.anchoredPosition = new Vector2(i * 45f + xOffset, yOffset);
            rect.localRotation = Quaternion.Euler(0, 0, randomZRotation);
            rect.localScale = Vector3.zero;

            // Animate scale up with delay
            rect.DOScale(Vector3.one, 0.2f)
                .SetEase(Ease.OutBack)
                .SetDelay(i * 0.15f);

            SceneController.instance.audioManager.PlaySfx(bulletSfx);
            bulletUIList.Add(bullet);
        }
    }
    public void ClearAmmoUI()
    {
        if (!uiCanvas || !bulletUI) return;

        uiCanvas.GetComponentInParent<Image>().sprite = gunUI;
        foreach (var b in bulletUIList)
        {
            Destroy(b);
        }
        bulletUIList.Clear();
    }

    public bool isPickup = false;
    public float swingAngle = 15f;
    public float swingDuration = 2;
    public void StartSwing()
    {
        if (!isPickup) return;

        transform.rotation = Quaternion.identity;
        Sequence swingSeq = DOTween.Sequence();

        bool startRight = Random.value > 0.5f;

        if (startRight)
        {
            swingSeq.Append(transform.DORotate(new Vector3(0, 0, swingAngle), swingDuration).SetEase(Ease.InOutSine));
            swingSeq.Append(transform.DORotate(new Vector3(0, 0, -swingAngle), swingDuration).SetEase(Ease.InOutSine));
            swingSeq.Append(transform.DORotate(new Vector3(0, 0, swingAngle), swingDuration).SetEase(Ease.InOutSine));
            swingSeq.Append(transform.DORotate(Vector3.zero, swingDuration).SetEase(Ease.InOutSine));
        }
        else
        {
            swingSeq.Append(transform.DORotate(new Vector3(0, 0, -swingAngle), swingDuration).SetEase(Ease.InOutSine));
            swingSeq.Append(transform.DORotate(new Vector3(0, 0, swingAngle), swingDuration).SetEase(Ease.InOutSine));
            swingSeq.Append(transform.DORotate(new Vector3(0, 0, -swingAngle), swingDuration).SetEase(Ease.InOutSine));
            swingSeq.Append(transform.DORotate(Vector3.zero, swingDuration).SetEase(Ease.InOutSine));
        }

        swingSeq.SetLoops(-1);
    }
}

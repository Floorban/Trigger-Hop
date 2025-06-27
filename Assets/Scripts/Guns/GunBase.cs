using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

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
    public float bulletSpeed = 20f;
    public float recoilForce = 5f;
    [SerializeField] protected float bulletLifetime = 1f;
    public int baseDamage;

    [Header("UI")]
    [SerializeField] private RectTransform uiCanvas;
    [SerializeField] protected GameObject bulletUI;
    protected List<GameObject> bulletUIList = new List<GameObject>();

    [Header("References")]
    [SerializeField] protected GameObject bulletPrefab;
    public Transform shootPoint;

    protected int currentAmmo;
    public bool canShoot = false;
    protected float lastShotTime;
    protected bool isReloading = false;
    [HideInInspector] public  Vector2 aimDir;
    private void Awake() {
        uiCanvas = GameObject.Find("Ammo").GetComponent<RectTransform>();
        currentAmmo = clipSize;
        inputAimDIr = GetInputDir(reverseAimDir);
    }
    public virtual void Setup(PlayerController p) {
        player = p;
        inputAimDIr = GetInputDir(reverseAimDir);
        lastShotTime = -1;
        Debug.Log(name + " in hand");
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
            UpdateAmmoUI();
            Debug.Log(currentAmmo + " left");
        }

        if (fireMode != FireMode.Auto) {
            canShoot = false;
            Invoke(nameof(ResetCooldown), cooldown);
        }

        if (currentAmmo <= 0) {
            canShoot = false;
            if (autoReload) {
                Reload();
            }
        }

        // activate cooldown
        lastShotTime = Time.time;
        return true;
    }
    protected void ResetCooldown() {
        canShoot = true;
    }
    public void Reload() {
        if (isReloading) return;
        isReloading = true;
        Debug.Log("reloading");
        // add anim and sfx here
        // TO DO: get the reload duration from each gun's scriptable stats 
        Invoke(nameof(FinishReload), 1f);
    }
    protected void FinishReload() {
        currentAmmo = clipSize;
        isReloading = false;
        ReloadAmmoUI();
        Debug.Log("finish reload");
    }
    protected void UpdateAmmoUI()
    {
        if (bulletUIList == null || bulletUIList.Count == 0) return;

        int indexToRemove = currentAmmo;
        if (indexToRemove >= 0 && indexToRemove < bulletUIList.Count)
        {
            GameObject bullet = bulletUIList[indexToRemove];

            // Animate out or hide
            bullet.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack)
                .OnComplete(() => bullet.SetActive(false));
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
            float yOffset = Random.Range(-10f, 10f);
            float randomZRotation = Random.Range(-10f, 10f);

            rect.anchoredPosition = new Vector2(i * 45f + xOffset, yOffset);
            rect.localRotation = Quaternion.Euler(0, 0, randomZRotation + 90f);

            rect.localScale = Vector3.zero;

            // Animate scale up with delay
            rect.DOScale(Vector3.one, 0.2f)
                .SetEase(Ease.OutBack)
                .SetDelay(i * 0.15f);

            bulletUIList.Add(bullet);
            canShoot = true;
        }
    }
}

using UnityEngine;

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

    [Header("References")]
    [SerializeField] protected GameObject bulletPrefab;
    public Transform shootPoint;

    protected int currentAmmo;
    public bool canShoot = true;
    protected float lastShotTime;
    protected bool isReloading = false;
    [HideInInspector] public  Vector2 aimDir;
    private void Awake() {
        canShoot = true;
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
        canShoot = true;
        Debug.Log("finish reload");
    }
}

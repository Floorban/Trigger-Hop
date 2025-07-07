using DG.Tweening;
using UnityEngine;

public abstract class GunController : GunBase
{
    public abstract void OnTouchBegin(Vector2 screenPos);
    public abstract void OnTouchDrag(Vector2 screenPos);
    public abstract void OnTouchEnd();
    public abstract void ShootProjectile(Vector2 direction);
    public abstract int AmmoCostPerShot { get; }
    public abstract FireMode fireMode { get; }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPickup || !collision.CompareTag("Player")) return;

        isPickup = false;
        GetComponent<Collider2D>().isTrigger = false;
        var weapon = FindFirstObjectByType<WeaponManager>();
        weapon.allGuns.Add(this);
        DOTween.KillAll();
        gameObject.transform.SetParent(weapon.transform);
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.SetActive(false);
    }
}

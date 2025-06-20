  using UnityEngine;

public class Pistol : GunController
{
    public override int AmmoCostPerShot => 1;
    public override FireMode fireMode => FireMode.Manual;
    public override void OnTouchBegin(Vector2 screenPos) {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        aimDir = (worldPos - (Vector2)transform.position);
    }
    public override void OnTouchDrag(Vector2 screenPos) {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        aimDir = (worldPos - (Vector2)transform.position);
    }
    public override void OnTouchEnd() {
        if (!TryFire(AmmoCostPerShot, fireMode)) return;

        ShootProjectile(inputAimDIr * aimDir);
        player.ApplyRecoil(inputAimDIr * aimDir, recoilForce);
    }
    public override void ShootProjectile(Vector2 direction) {
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        AttackComponent bac = bullet.AddComponent<AttackComponent>();
        bac.damageAmount = baseDamage;
        Rigidbody2D brb = bullet.GetComponent<Rigidbody2D>();
        brb.constraints = RigidbodyConstraints2D.FreezeRotation;
        brb.linearVelocity = direction.normalized * bulletSpeed;
        Destroy(bullet, bulletLifetime);
    }
}

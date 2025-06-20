using UnityEngine;

public class LaserGun : GunController {
    public override int AmmoCostPerShot => 0;
    public override FireMode fireMode => FireMode.Charge;
    public override void OnTouchBegin(Vector2 screenPos) {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        aimDir = (worldPos - (Vector2)transform.position);
    }
    public override void OnTouchDrag(Vector2 screenPos) {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        aimDir = (worldPos - (Vector2)transform.position);
    }
    public override void OnTouchEnd() {
        if (!TryFire(AmmoCostPerShot, fireMode))
            return;

        //ShootProjectile(stats.inputAimDIr * aimDir);
        //player.ApplyRecoil(stats.inputAimDIr * aimDir, stats.recoilForce);
    }
    public override void ShootProjectile(Vector2 direction) {
        throw new System.NotImplementedException();
    }
}

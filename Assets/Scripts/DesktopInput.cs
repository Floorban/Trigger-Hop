using UnityEngine;

public class DesktopInput : IInput {
    public void HandleInput(WeaponManager wManager) {
        HandleShootInput(wManager);
        HandleReloadInput(wManager);
        HandleSwitchWeaponInput(wManager);
    }
    private void HandleShootInput(WeaponManager wManager) {
        if (wManager.currentGun) {
            GunController gun = wManager.currentGun;
            if (Input.GetMouseButtonDown(0)) {
                gun.OnTouchBegin(Input.mousePosition);
                wManager.StartAiming();
            }
            else if (Input.GetMouseButton(0)) {
                gun.OnTouchDrag(Input.mousePosition);
                wManager.UpdateAimLine(gun.aimDir);
            }
            else if (Input.GetMouseButtonUp(0)) {
                gun.OnTouchEnd();
                wManager.StopAiming();
            }
        }
    }
    private void HandleReloadInput(WeaponManager wManager) {
        if (wManager.currentGun) {
            GunController gun = wManager.currentGun;
            if (Input.GetKeyDown(KeyCode.R))
                gun.Reload();
        }
    }
    private void HandleSwitchWeaponInput(WeaponManager wManager) {
        if (Input.mouseScrollDelta.y != 0)
            wManager.CycleGun(Input.mouseScrollDelta.y);
    }
}

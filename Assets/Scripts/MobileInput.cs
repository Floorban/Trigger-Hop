using UnityEngine;

public class MobileInput : IInput {
    public void HandleInput(WeaponManager wManager) {
        HandleShootInput(wManager);
        HandleReloadInput(wManager);
        HandleSwitchWeaponInput(wManager);
    }
    private void HandleShootInput(WeaponManager wManager) {
        if (Input.touchCount == 1 && !wManager.isSwipingTwoFingers) {
            Touch touch = Input.GetTouch(0);

            if (touch.position.y > Screen.height * 0.8f || touch.position.y < Screen.height * 0.1f)
                return;

            if (wManager.currentGun) {
                GunController gun = wManager.currentGun;
                switch (touch.phase) {
                    case TouchPhase.Began:
                        gun.OnTouchBegin(touch.position);
                        wManager.StartAiming();
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        gun.OnTouchDrag(touch.position);
                        wManager.UpdateAimLine(gun.aimDir);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        gun.OnTouchEnd();
                        wManager.StopAiming();
                        SceneController.instance.cam.Shake();
                        break;
                }
            }
        }
        else if (Input.touchCount != 1){
            wManager.StopAiming();
        }
    }
    private void HandleReloadInput(WeaponManager wManager) {
        Vector3 acc = Input.acceleration;
        if (acc.sqrMagnitude > wManager.shakeIntensity) {
            wManager.currentGun.Reload(wManager.currentGun.reloadDuration);
        }
    }
    private void HandleSwitchWeaponInput(WeaponManager wManager) {
        if (Input.touchCount == 2) {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            if (t1.phase == TouchPhase.Began || t2.phase == TouchPhase.Began) {
                wManager.swipeStartPos = (t1.position + t2.position) / 2f;
                wManager.isSwipingTwoFingers = true;
            }
            else if ((t1.phase == TouchPhase.Ended || t1.phase == TouchPhase.Canceled) &&
                          (t2.phase == TouchPhase.Ended || t2.phase == TouchPhase.Canceled) &&
                           wManager.isSwipingTwoFingers) {
                Vector2 swipeEndPos = (t1.position + t2.position) / 2f;
                Vector2 swipeDelta = swipeEndPos - wManager.swipeStartPos;

                if (Mathf.Abs(swipeDelta.y) > wManager.cycleDistance) {
                    wManager.CycleGun(swipeDelta.y);
                }

                wManager.isSwipingTwoFingers = false;
            }
        }
        else {
            wManager.isSwipingTwoFingers = false;
        }
    }
}

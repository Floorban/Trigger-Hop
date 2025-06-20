using UnityEngine;

public abstract class GunController : GunBase
{
    public abstract void OnTouchBegin(Vector2 screenPos);
    public abstract void OnTouchDrag(Vector2 screenPos);
    public abstract void OnTouchEnd();
    public abstract void ShootProjectile(Vector2 direction);
    public abstract int AmmoCostPerShot { get; }
    public abstract FireMode fireMode { get; }
}

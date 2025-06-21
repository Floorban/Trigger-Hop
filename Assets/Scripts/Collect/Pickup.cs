using UnityEngine;

public abstract class Pickup : MonoBehaviour, ICollectible
{
    public Transform target;
    public float moveSpeed = 5f;

    public abstract void Collect();

    private void FixedUpdate()
    {
        if (target)
        {
            float distance = Vector2.Distance(transform.position, target.position);
            float lerpFactor = Mathf.Clamp01(moveSpeed * Time.fixedDeltaTime / distance);
            transform.position = Vector2.Lerp(transform.position, target.position, lerpFactor);
        }
    }
}

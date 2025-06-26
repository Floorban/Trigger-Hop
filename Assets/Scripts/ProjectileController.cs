using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float maxLifetime;
    private float currentLifetime;
    private Rigidbody2D rb;

    public void Init(Vector2 dir, float spd, float lifetime)
    {
        direction = dir.normalized;
        speed = spd;
        maxLifetime = lifetime;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * speed * Time.fixedDeltaTime;
        currentLifetime += Time.fixedDeltaTime;
        if (currentLifetime >= maxLifetime) Destroy(gameObject);
    }
}

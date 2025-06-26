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
        float timeScale = SceneController.instance.GetPlayerTime();
        rb.linearVelocity = direction * speed * timeScale;
        currentLifetime += Time.fixedDeltaTime * timeScale;
        if (currentLifetime >= maxLifetime)
            Destroy(gameObject);
    }
}

using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float maxLifetime;
    private float currentLifetime;
    private Rigidbody2D rb;
    [SerializeField] private GameObject physBullet, colEffect;

    public void Init(Vector2 dir, float spd, float lifetime, bool isPhysical)
    {
        direction = dir.normalized;
        speed = spd;
        maxLifetime = lifetime;

        if (!isPhysical) return;
        float randomZ = Random.Range(-10f, 10f);
        Quaternion randomRotation = Quaternion.Euler(0f, 0f, randomZ);
        GameObject bul = Instantiate(physBullet, transform.position, randomRotation);
        Rigidbody2D rb = bul.GetComponent<Rigidbody2D>();

        float angleOffset = Random.Range(-5f, 5f);
        Vector2 forceDirection = Quaternion.Euler(0, 0, angleOffset) * Vector2.up;
        float forcePower = 1f;

        Vector2 localRandomPoint = Random.insideUnitCircle * 0.5f; // radius from center
        Vector2 worldPoint = rb.transform.TransformPoint(localRandomPoint);
        rb.AddForceAtPosition(forceDirection * forcePower, worldPoint, ForceMode2D.Impulse);
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Instantiate(colEffect, transform.position, transform.rotation);
            Destroy(gameObject, 0.1f);
        }
    }
}

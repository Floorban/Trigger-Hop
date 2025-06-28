using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float maxLifetime;
    private float currentLifetime;
    private Rigidbody2D rb;
    [SerializeField] private GameObject physBullet, colEffect;

    public void Init(Vector2 dir, float spd, float lifetime)
    {
        direction = dir.normalized;
        speed = spd;
        maxLifetime = lifetime;
        GameObject bul = Instantiate(physBullet, transform.position, transform.rotation);
        bul.GetComponent<Rigidbody2D>().AddForce(Vector2.up, ForceMode2D.Impulse);
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

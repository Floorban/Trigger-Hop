using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(HealthManager))]
[RequireComponent(typeof(Collider2D))]
public class EnemyBase : MonoBehaviour, IDamageable
{
    public EnemyStats stats;
    protected HealthSystem health;
    public bool canBeDamaged = true;
    [SerializeField] protected float coolDown = 1f;
    protected float timer;
    public GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    public bool active;

    [Header("Anim Settings")]
    public float squashAmount = 0.8f;
    public float stretchAmount = 1.1f;
    public float squashDuration = 0.2f;
    public float holdDuration = 0.05f;
    public float returnDuration = 0.2f;

    private Vector3 originalScale;
    private Sequence squashLoop;
    public bool Active
    {
        get => active;
        set
        {
            if (active == value) return;
            active = value;

            if (value)
                StartSquashLoop();
            else
                StopSquashLoop();
        }
    }
    private void Start()
    {
        originalScale = transform.localScale;
        health = GetComponent<HealthManager>().healthSystem;
        Active = true;
    }

    private void FixedUpdate()
    {
        if (!active) return;

        if (timer < coolDown)
        {
            timer += Time.fixedDeltaTime;
        }
        else
        {
            timer = 0;
            Shoot();
        }
    }
    public void Shoot()
    {
        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        AttackComponent bac = proj.AddComponent<AttackComponent>();
        bac.damageAmount = 1;
        Rigidbody2D brb = proj.GetComponent<Rigidbody2D>();
        brb.constraints = RigidbodyConstraints2D.FreezeRotation;
        proj.GetComponent<ProjectileController>().Init(Vector2.right, 1f, 5f, false);
    }
    public virtual void Die() {
        Destroy(gameObject, 0f);
    }
    public void TakeDamage(int damageAmount, float stun)
    {
        if (!canBeDamaged) return;

        health.Damage(damageAmount);

        if (health.GetHealth() <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(Invincible(stats.invincibleTime));
            StartCoroutine(StunRecovery(stun));
        }
    }
    private IEnumerator Invincible(float duration) {
        if (canBeDamaged) {
            canBeDamaged = false;
            float timestep = 0;
            while (timestep < duration) {
                timestep += Time.deltaTime;
            }
            yield return new WaitForSeconds(duration);
            canBeDamaged = true;
        }
    }
    private IEnumerator StunRecovery(float duration) {
        active = false;
        yield return new WaitForSeconds(duration);
        active = true;
    }

    private void StartSquashLoop()
    {
        StopSquashLoop();

        squashLoop = DOTween.Sequence();
        squashLoop.Append(transform.DOScale(
                new Vector3(originalScale.x * stretchAmount, originalScale.y * squashAmount, originalScale.z),
                squashDuration
            ).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(originalScale, squashDuration).SetEase(Ease.InOutQuad))
            .AppendInterval(holdDuration)
            .SetLoops(-1);
    }
    public void StopSquashLoop()
    {
        if (squashLoop != null && squashLoop.IsActive())
        {
            squashLoop.Kill();
            squashLoop = null;
            transform.DOScale(originalScale, returnDuration).SetEase(Ease.OutElastic);
        }
    }
}

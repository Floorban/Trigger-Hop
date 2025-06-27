using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HealthManager))]
[RequireComponent(typeof(Collider2D))]
public class EnemyBase : MonoBehaviour, IDamageable
{
    public EnemyStats stats;
    protected HealthSystem health;
    public bool canBeDamaged = true;
    public bool canAct = true;
    [SerializeField] protected float coolDown = 1f;
    protected float timer;
    public GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    
    public void TakeDamage(int damageAmount, float stun) {
        if (!canBeDamaged) return;

        health.Damage(damageAmount);

        if (health.GetHealth() <= 0) {
            Die();
        }
        else {
            StartCoroutine(Invincible(stats.invincibleTime));
            StartCoroutine(StunRecovery(stun));
        }
    }

    private void Start() {
        health = GetComponent<HealthManager>().healthSystem;
    }
    private void FixedUpdate()
    {
        if (!canAct) return;

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
        ProjectileController bulletCtrl = proj.AddComponent<ProjectileController>();
        bulletCtrl.Init(Vector2.left, 1f, 5f);
    }
    public virtual void Die() {
        Destroy(gameObject, 0f);
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
        canAct = false;
        yield return new WaitForSeconds(duration);
        canAct = true;
    }
}

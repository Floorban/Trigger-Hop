using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HealthManager))]
[RequireComponent(typeof(Collider2D))]
public class EnemyBase : MonoBehaviour, IDamageable
{
    public EnemyStats stats;
    protected HealthSystem health;
    public bool canBeDamaged = true;
    protected bool canAct = true;
    
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

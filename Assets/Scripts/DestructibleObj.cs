using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(HealthManager))]
public class DestructibleObj : MonoBehaviour, IDamageable
{
    private Collider col;
    private HealthSystem healthSystem;
    private bool canBeDamaged = true;

    private void Awake()
    {
        col = GetComponent<Collider>();
        healthSystem = GetComponent<HealthManager>().healthSystem;
        canBeDamaged = true;
    }
    public void TakeDamage(int damageAmount, float stun)
    {
        if (!canBeDamaged) return;

        healthSystem.Damage(1); // so the level obj only takes 1 damage like the ward in the league: https://leagueoflegends.fandom.com/wiki/Ward

        if (healthSystem.GetHealth() == 0)
        {
            Die();
            return;
        }
    }
    void Die()
    {
        Destroy(gameObject, 0f);
        col.enabled = false;
        canBeDamaged = false;
    }
}

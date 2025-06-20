using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackComponent : MonoBehaviour
{
    public int damageAmount;
    public float stunTime;
    //public ParticleSystem hitParc;

    private void OnTriggerEnter2D(Collider2D collision) {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null) {
            damageable.TakeDamage(damageAmount, stunTime);
            Destroy(gameObject);
        }
        // TO DO: get the contact point and spawn particle
        //Destroy(gameObject);
    }
}

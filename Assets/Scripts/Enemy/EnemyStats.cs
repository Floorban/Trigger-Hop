using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Enemy")]
public class EnemyStats : ScriptableObject
{
    [Header("COMBAT")]
    public float invincibleTime = 0.5f;
    public int damage = 1;
    public float projectileSpeed = 3f;
    public LayerMask attackLayer;
}

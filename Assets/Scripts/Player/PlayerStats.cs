using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Player")]
public class PlayerStats : ScriptableObject
{
    [Header("LAYERS")]
    [Tooltip("Set this to the layer of the player")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    [Header("GROUNDER SETTINGS")]
    [Tooltip("Amount to offset ground/ceiling detection and range of detection sphere")]
    public float grounderOffset = -0.87f;
    public float grounderRadius = 0.2f;

    [Header("WALL CHECK OFFSET")]
    [Tooltip("Amount to offset wall detection and range of detection sphere")]
    public float wallCheckOffset = 0.2f;
    public float wallCheckRadius = 0.2f;

    [Header("COMBAT")]
    public float invincibleTime = 0.5f;

    [Header("WALKING SPEED")]
    [Tooltip("How fast the player moves gets to max speed. Velocity adjustments for acceleration.")]
    public float walkSpeed = 10f;
    public float acceleration = 1.43f;
    public float maxWalkingPenalty = 0.5f;
    public float currentMovementLerpSpeed = 100;

}

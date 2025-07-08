using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Attached to GameObject that has health
public class HealthManager : MonoBehaviour
{
    public int maxHealth;
    public HealthSystem healthSystem;

    private void Awake() => healthSystem = new HealthSystem(maxHealth);

}

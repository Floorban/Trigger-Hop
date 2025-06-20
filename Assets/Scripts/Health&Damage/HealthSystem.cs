using System;
using System.Collections.Generic;
using UnityEngine;

// This script does not get attached to a GameObject
// It lives in the scripts folder, and new instances are created via HealthManager.cs
public class HealthSystem 
{
    public event EventHandler OnHealthChanged;
    public event EventHandler OnHealed;
    public event EventHandler OnArmorChanged;
    public event EventHandler OnArmorUp;
    private float health;
    public float healthMax;

    private int armor;
    public int armorMax;
    public bool armored;

    // Creates a new instance of a health system.
    // Can be used for player, objects, NPCs, enemies since each instance is independent.
    public HealthSystem(float maxHealth, int maxArmor)
    {
        this.healthMax = maxHealth;
        health = healthMax;
        this.armorMax = maxArmor;
    }
    #region Health
    // this can be used for instances needing to return player health
    // UI, debugging, etc.
    public float GetHealth() => health;

    // Health percent is used in healthbar UI
    public float GetHealthPercent()
    {
        var percent = health / healthMax;
        return percent;
    }

  
    // reduces health and updates the healthbar UI
    // this can be called from the attacking gameobject with logic of:
    //  // have I hit something?
    //  // does it have a HealthSystem component?
    //  // reference HealthSystem component Damage()
    public void Damage(int damageAmt)
    {
        var dmg = damageAmt;
        if (armored) 
            dmg = DamageArmor(damageAmt);
        
        health -= dmg;
        if (health < 0) health = 0;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);

    }

    // increases health and updates the healthbar UI.
    public void Heal (int healAmt)
    {
        health += healAmt;
        if (health > healthMax) health = healthMax;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
        OnHealed?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region Armor
    public int GetArmor() => armor;
    public float GetArmorPercent()
    {
        var percent = armor / armorMax;
        return percent;
    }
    public int DamageArmor(int damageAmt)
    {
        // return full damage amount if no armor
        if (!armored)
            return damageAmt;

        // if damage is greater than armor, return the difference
        if(damageAmt >= armor)
        {
            var overflow = Mathf.Abs(armor - damageAmt);

            armor = 0;
            armored = false;
            OnArmorChanged?.Invoke(this, EventArgs.Empty);
            return overflow;
        }
        else
        {
            // if equal or greater armor than damage, return no damage to player
            armor -= damageAmt;
            OnArmorChanged?.Invoke(this, EventArgs.Empty);
            return 0;
        }
    }

    public void ArmorUp(int armorAmt)
    {
        armor += armorAmt;
        if (armor > armorMax) armor = armorMax;
        if (armor > 0) armored = true;
        OnArmorChanged?.Invoke(this, EventArgs.Empty);
        OnArmorUp?.Invoke(this, EventArgs.Empty);
    }

    #endregion
}

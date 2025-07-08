using System;

public class HealthSystem 
{
    public event EventHandler OnHealthChanged;
    public event EventHandler OnHealed;
    private float health;
    public float healthMax;

    public HealthSystem(float maxHealth)
    {
        this.healthMax = maxHealth;
        health = healthMax;
    }
    #region Health
    public float GetHealth() => health;

    public float GetHealthPercent()
    {
        var percent = health / healthMax;
        return percent;
    }
    public void Damage(int damageAmt)
    {
        var dmg = damageAmt;
        health -= dmg;
        if (health < 0) health = 0;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }
    public void Heal (int healAmt)
    {
        health += healAmt;
        if (health > healthMax) health = healthMax;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
        OnHealed?.Invoke(this, EventArgs.Empty);
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Attached to GameObject that has health
public class HealthManager : MonoBehaviour
{
    public int maxHealth;

    [Header("Armor/Shield")]
    public GameObject armorPrefab;
    public int maxArmor;
    public int startArmor;
    public Image armorUI;

    [Header("Bar System")]
    public Image healthUI; // fill image for health UI, fill amount set via event

    [Header("Lives System")]
    public bool isLives;
    public GameObject healthContainer;
    public GameObject heartPrefab;
    private List<GameObject> lives;
    
    [HideInInspector]public HealthSystem healthSystem;

    private void Awake()
    {
        // create new health system for object this script is attached to.
        // integer is health max value and armor max value
        healthSystem = new HealthSystem(maxHealth, maxArmor);
        LivesSetup();
        // set if system starts with armor
        ArmorSetup();
        Armor();
    }
    void Start()
    {
        // subscribes to the event
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        healthSystem.OnArmorChanged += HealthSystem_OnArmorChanged;
    }
    void ArmorSetup()
    {
        healthSystem.ArmorUp(startArmor);
        if (healthSystem.GetArmor() > 0) healthSystem.armored = true;
    }
    void Armor()
    {
        if (armorUI) armorUI.fillAmount = healthSystem.GetArmorPercent();
        if (armorPrefab)
            if (healthSystem.armored) armorPrefab.SetActive(true);
            else armorPrefab.SetActive(false);

    }
    void LivesSetup()
    {
        lives = new List<GameObject>();
        var hearts = healthSystem.healthMax;
        if(isLives)
        {
            for (int i = 0; i < hearts; i++)
            {
                var heart = Instantiate(heartPrefab, healthContainer.transform);
                lives.Add(heart);
                Debug.LogWarning("Made heart!");
            }
            LivesUpdate();
        }
    }
    void LivesUpdate()
    {
        if (isLives)
        {
            var health = healthSystem.GetHealth();
            for (int i = 0; i < lives.Count; i++)
            {
                var heart = lives[i].GetComponent<HealthLives>();
                if (i+1 > health)
                {
                    heart.SetEmpty();
                    continue;
                }
                else if(i+1 < health)
                {
                    heart.SetFull();
                    continue;
                }
/*                else
                {
                    bool isEven = healthSystem.GetHealth() % 2 == 0;
                    if (isEven) heart.SetFull();
                    else heart.SetHalf();
                }*/
            }
        }
    }
    private void HealthSystem_OnArmorChanged(object sender, System.EventArgs e)
    {
        Armor();
    }

    private void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        // updates the healthbar UI to current health percent of max
        if(healthUI && !isLives) healthUI.fillAmount = healthSystem.GetHealthPercent();
        if (isLives && healthContainer) LivesUpdate();
        
    }
    private void OnDestroy()
    {
        healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
        healthSystem.OnArmorChanged -= HealthSystem_OnArmorChanged;
    }

    public void IncreaseLives()
    {
        healthSystem.healthMax += 2;
        LivesUpdate();
    }
}

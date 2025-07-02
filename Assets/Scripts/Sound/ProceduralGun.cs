using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor.ShaderGraph.Internal;

using FMODUnity;
using FMOD.Studio;

[CreateAssetMenu(fileName = "Procedural Gun Sound", menuName = "Procedural Gun", order = 1)]
public class ProceduralGun : ScriptableObject
{
    public List<float> ShotParameters { get; private set; }

    [Header("Gun Settings")]
    [Tooltip("The type of gun. For reference: Pistol, Rifle, Shotgun")]
    [SerializeField] EventReference GunSound;

    [Header("Ammunition Settings")]
    [Tooltip("The type of ammunition. Bullet is regular ammunition while light creates ray gun-like sounds as it uses light")]
    [SerializeField] public AmmunitionType ammunition = AmmunitionType.Bullet; 
    [SerializeField] public bool randomizeAmmunition = false;

    [Tooltip("The size of the caliber in millimeters")]
    [Range(1, 30)][SerializeField] public float ammoSize = 9f;
    [SerializeField] public bool randomizeAmmoSize = false;

    [Tooltip("The weight of the caliber in kilograms. For reference: Pistol = 0.2g, ")]
    [Range(1, 20)][SerializeField] public float ammoWeight = 9f;
    [SerializeField] public bool randomizeAmmoWeight = false;

    [Header("Barrel Settings")]
    [Tooltip("The type of barrel. Muzzle Break sharpens the sound since it gets redirected to the shooter, while suppressor filters ear-sensitive frequencies to camoflauge the gunshot sound")]
    [SerializeField] public MuzzleType muzzle = MuzzleType.Normal;
    [SerializeField] public bool randomizeMuzzle = false;

    [Tooltip("The length of the barrel in centimeters. For reference: Pistol = 10cm, Rifle = 65cm, Shotgun = 70cm")]
    [Range(1, 100)][SerializeField] public float barrelLength = 15f;
    [SerializeField] public bool randomizeBarrelLength = false;

    [Tooltip("The diameter of the barrel in centimeters. For reference: Pistol = 1cm, Rifle = 2.5cm, Shotgun = 4cm")]
    [Range(1, 25)][SerializeField] public float barrelDiameter  = 9f;
    [SerializeField] public bool randomizeBarrelDiameter = false;

    [HideInInspector] public enum AmmunitionType { Bullet, Laser };
    [HideInInspector] public enum MuzzleType { Normal, MuzzleBreak, Suppressor };

    private EventInstance i_GunSound;

    public void RandomizeAttributes() {
        if (randomizeAmmoSize) ammoSize = UnityEngine.Random.Range(1f, 30f);
        if (randomizeAmmoWeight) ammoWeight = UnityEngine.Random.Range(1f, 20f);
        if (randomizeBarrelDiameter) barrelDiameter = UnityEngine.Random.Range(1f, 25f);
        if (randomizeBarrelLength) barrelLength = UnityEngine.Random.Range(1f, 100f);

        if (randomizeAmmunition) {
            int ammunitionType = UnityEngine.Random.Range(0, Enum.GetNames(typeof(AmmunitionType)).Length);
            ammunition = (AmmunitionType)ammunitionType;
        }

        if (randomizeMuzzle) {
            int muzzleType = UnityEngine.Random.Range(0, Enum.GetNames(typeof(MuzzleType)).Length);
            muzzle = (MuzzleType)muzzleType;
        }

    }

    public void PlaySound(Vector3 playPosition) {
        if (!GunSound.IsNull) {
            i_GunSound = AudioManager.Instance.CreateInstance(GunSound, playPosition);

            i_GunSound.setParameterByNameWithLabel("Ammunition", Enum.GetName(typeof(AmmunitionType), ammunition));
            i_GunSound.setParameterByName("Ammo Size", ammoSize);
            i_GunSound.setParameterByName("Ammo Weight", ammoWeight);
            i_GunSound.setParameterByNameWithLabel("Muzzle", Enum.GetName(typeof(MuzzleType), muzzle));
            i_GunSound.setParameterByName("BarrelLength", barrelLength);
            i_GunSound.setParameterByName("BarrelDiameter", barrelDiameter);

            i_GunSound.start();
            i_GunSound.release();
        }
        else Debug.Log("Sound not found: " + GunSound);
    }
    public void RandomizeSound(Vector3 playPosition) {
        RandomizeAttributes();
        if (!GunSound.IsNull) {
            i_GunSound = AudioManager.Instance.CreateInstance(GunSound, playPosition);

            i_GunSound.setParameterByNameWithLabel("Ammunition", Enum.GetName(typeof(AmmunitionType), ammunition));
            i_GunSound.setParameterByName("Ammo Size", ammoSize);
            i_GunSound.setParameterByName("Ammo Weight", ammoWeight);
            i_GunSound.setParameterByNameWithLabel("Muzzle", Enum.GetName(typeof(MuzzleType), muzzle));
            i_GunSound.setParameterByName("BarrelLength", barrelLength);
            i_GunSound.setParameterByName("BarrelDiameter", barrelDiameter);

            i_GunSound.start();
            i_GunSound.release();
        }
        else Debug.Log("Sound not found: " + GunSound);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponMod_", menuName = "Weapon/WeaponMod")]
public class WeaponMod : ScriptableObject
{
    [SerializeField] string weaponModName = "Default";
    [SerializeField] string weaponPickupText = "Default";
    [TextArea] [SerializeField] string weaponModDescription;

    [Header("Weapon Emission Color")]
    [SerializeField] Color emissionColor = Color.white;

    [Header("Blur Settings")]
    [SerializeField] AnimationCurve screenBlurSize;
    [SerializeField] [Range(0.0001f, 0.01f)] float screenBlurSizeIncrement;
    [SerializeField] [Range(0.0001f, 0.01f)] float screenBlurCap = 0.01f;
    [SerializeField] float screenBlurTime = 0.3f;

    [Header("Suppression Settings")]
    [SerializeField] AnimationCurve suppression;
    [SerializeField] [Range(0.001f, 1f)] float suppressionIncrement;
    [SerializeField] float suppressionRecoveryDelay = 1;


    [Header("In-Game Weapon Stats")]
    [SerializeField] FireBehavior fireBehavior;
    [SerializeField] WeaponStats weaponStats;

    [FMODUnity.EventRef][Space]
    public string installationSound;

    public string WeaponModName { get { return weaponModName; } }
    public string WeaponPickupText { get { return weaponPickupText; } }
    public string WeaponModDescription { get { return weaponModDescription; } }

    public Color EmissionColor { get { return emissionColor; } }

    public float ScreenBlurCap { get { return screenBlurCap; } }
    public float ScreenBlurSizeIncrement { get { return screenBlurSizeIncrement; } }
    public float ScreenBlurTime { get { return screenBlurTime; } }
    public AnimationCurve ScreenBlurSize { get { return screenBlurSize; } }

    public float SuppressionIncrement { get { return suppressionIncrement; } }
    public float SuppressionRecoveryDelay { get { return suppressionRecoveryDelay; } }
    public AnimationCurve Suppression { get { return suppression; } }

    public FireBehavior FireBehavior { get { return fireBehavior; } }
    public WeaponStats WeaponStats { get { return weaponStats; } }
}

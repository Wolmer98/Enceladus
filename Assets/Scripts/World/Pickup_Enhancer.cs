using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds functinality for both "medcation" and "suitmods".
/// </summary>
public class Pickup_Enhancer : Pickup
{
    [SerializeField] bool isSuitMod;
    [SerializeField] CharacterScreenOption suitModOption;

    [Space]
    [SerializeField] bool enableFlashlight;
    [SerializeField] int keycards;

    [Space]
    [SerializeField] public float ammo;
    [SerializeField] public float maxAmmoStorage;
    [SerializeField] public float health;
    [SerializeField] public float maxHealth;
    [SerializeField] public float regeneration;
    [SerializeField] public float resistance;
    [SerializeField] public float speed;

    public CharacterScreenOption SuitModOption { get { return suitModOption; } }

    void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Adds the picked up values to the player.
    /// </summary>
    public override bool PickUp(PlayerController pc, bool pickUpIsPrefab = false)
    {
        if (isSuitMod && AlreadyEquippedSuitMod(suitModOption, pc))
        {
            Debug.Log("Suit mod Already Equipped");
            return false;
        }

        pc.MainWeapon.AddMaxStorageAmmo(maxAmmoStorage);

        if (pc.MainWeapon.StorageAmmo >= pc.MainWeapon.MaxStorageAmmo && ammo > 0)
            return false;

        pc.MainWeapon.AddStorageAmmo(ammo);
        pc.Destructible.AddMaxHealth(maxHealth);
        pc.Destructible.Heal(health);
        pc.Destructible.AddRegeneration(regeneration);
        pc.Destructible.AddResistance(resistance);
        pc.FPSController.AddSpeed(speed);


        if (isSuitMod)
        {
            pc.AddSuitMod(suitModOption);
        }

        if (enableFlashlight)
        {
            pc.EnableFlashLight();
        }

        base.PickUp(pc, pickUpIsPrefab);
        return true;
    }

    private bool AlreadyEquippedSuitMod(CharacterScreenOption suitmod, PlayerController pc)
    {
        foreach (var suitMod in pc.SuitMods)
        {
            if (suitMod.OptionText.Equals(suitmod.OptionText))
                return true;
        }
        return false;
    }
}

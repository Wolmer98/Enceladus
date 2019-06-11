using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Weapon : Pickup
{
    [SerializeField] MeleeWeapon meleeWeapon;

    public MeleeWeapon SpawnedMeleeWeapon { get; private set; }

    /// <summary>
    /// Assigns the picked up item to the player.
    /// </summary>
    public override void PickUp(PlayerController pc, bool pickupIsPrefab = false)
    {
        if (pc.MeleeWeapon == null)
        {
            if (meleeWeapon != null)
            {
                pc.EquipWeapon(meleeWeapon, SpawnedMeleeWeapon.Durability);
            }

            if (pickupIsPrefab == false)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Sets the weapon of the pickup. Should only be done on instantiate.
    /// </summary>
    public void SetInitWeapon(MeleeWeapon meleeWeapon, int durability)
    {
        this.meleeWeapon = meleeWeapon;
        this.meleeWeapon.Durability = durability;
        InitPickup();
    }

    /// <summary>
    /// Helper function to acess SetWeaponLayers function in the pickup weapon.
    /// </summary>
    public void SetWeaponLayer(int layer)
    {
        if (SpawnedMeleeWeapon != null)
        {
            SpawnedMeleeWeapon.SetWeaponLayers(layer);
        }
    }

    protected override void InitPickup()
    {
        base.InitPickup();

        if (meleeWeapon != null)
        {
            SpawnedMeleeWeapon = Instantiate(meleeWeapon, ItemParent);
            EnableChildColliders(SpawnedMeleeWeapon.gameObject);
        }       
    }

    private void EnableChildColliders(GameObject parent)
    {
        foreach (Collider col in parent.GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
        }
    }
}

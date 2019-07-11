using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstallWeaponMod : MonoBehaviour
{
    WeaponMod weaponMod;

    [SerializeField] bool destroyOnActivation;

    [SerializeField] WeaponMod[] weaponMods;

    [FMODUnity.EventRef]
    public string installModSound;

    public WeaponMod WeaponMod
    {
        get
        {
            return weaponMod;
        }
    }

    private void Start()
    {
        if (weaponMods.Length > 0)
        {
            weaponMod = weaponMods[Random.Range(0, weaponMods.Length)];
        }
    }

    public WeaponMod GetWeaponModAndInstall()
    {
        WeaponMod wp = weaponMod;
        WeaponMod equippedMod = FindObjectOfType<Weapon>().MainWeaponMod;

        if (weaponMods.Length > 1)
        {
            while (wp.WeaponModName == equippedMod.WeaponModName)
            {
                if (weaponMods.Length > 0)
                {
                    wp = weaponMods[Random.Range(0, weaponMods.Length)];
                }
            }
        }

        weaponMod = null;
        GetComponent<GlowObject>().GlowColor = Color.black;
        GetComponent<GlowObject>().SetTargetColor(Color.black);

        if (GetComponentInChildren<Light>() != null)
        {
            GetComponentInChildren<Light>().color = Color.red;
        }

        FMODUnity.RuntimeManager.PlayOneShot(installModSound, transform.position);
        FMODUnity.RuntimeManager.PlayOneShot(wp.installationSound, transform.position);

        if (destroyOnActivation)
        {
            Destroy(gameObject, 0.2f);
        }

        return wp;
    }
}

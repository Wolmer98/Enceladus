using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Character", menuName = "Player/Character")]
public class CharacterData : ScriptableObject
{
    [SerializeField] string characterName;
    [SerializeField] string characterNameOnly;
    [SerializeField] string characterClassOnly;
    [SerializeField] Color characterColor = Color.white;
    [SerializeField] Sprite portrait;
    [TextArea]
    [SerializeField] string description;


    [Header("Weapon Settings")]
    [ShowIf("nonRandomStats")] [SerializeField] WeaponMod startWeaponMod;
    [ShowIf("nonRandomStats")] [SerializeField] int ammunition;
    [ShowIf("nonRandomStats")] [SerializeField] int maxStorageAmmo;

    [Space]
    [ShowIf("nonRandomStats")] [SerializeField] MeleeWeapon startMeleeWeapon;

    [Header("Starting SuitMods/Pickups")]
    [ShowIf("nonRandomStats")] [SerializeField] Pickup_Enhancer[] suitMods;

    [Header("Character Stats")]
    [ShowIf("nonRandomStats")] [SerializeField] float health;
    [ShowIf("nonRandomStats")] [SerializeField] float maxHealth;
    [ShowIf("nonRandomStats")] [SerializeField] float regeneration;
    [ShowIf("nonRandomStats")] [SerializeField] float resistance;
    [ShowIf("nonRandomStats")] [SerializeField] float speed;


    [Header("RNG Settings")]
    [SerializeField] bool randomStats = false;
    bool nonRandomStats = true;

    [Header("Weapon Settings")]
    [ShowIf("randomStats")] [ReorderableList] [SerializeField] WeaponMod[] RNG_startWeaponMod;
    [ShowIf("randomStats")] [SerializeField] Vector2 RNG_ammunition;
    [ShowIf("randomStats")] [SerializeField] Vector2 RNG_maxStorageAmmo;

    [Space]
    [ShowIf("randomStats")] [ReorderableList] [SerializeField] MeleeWeapon[] RNG_startMeleeWeapon;

    [Header("Starting SuitMods/Pickups")]
    [ShowIf("randomStats")] [SerializeField] Vector2 RNG_amountOfSuitMods;
    [ShowIf("randomStats")] [ReorderableList] [SerializeField] Pickup_Enhancer[] RNG_suitMods;

    [Header("Character Stats")]
    [ShowIf("randomStats")] [SerializeField] Vector2 RNG_health;
    [ShowIf("randomStats")] [SerializeField] Vector2 RNG_maxHealth;
    [ShowIf("randomStats")] [SerializeField] Vector2 RNG_regeneration;
    [ShowIf("randomStats")] [SerializeField] Vector2 RNG_resistance;
    [ShowIf("randomStats")] [SerializeField] Vector2 RNG_speed;


    [Header("Audio Settings")]
    [FMODUnity.EventRef]
    public string hurtSound;
    [FMODUnity.EventRef]
    public string deathSound;
    [FMODUnity.EventRef]
    public string sprintSound;
    [FMODUnity.EventRef]
    public string walkSound;

    public string CharacterName { get { return characterName; } }
    public string CharacterNameOnly { get { return characterNameOnly; } }
    public string CharacterClassOnly { get { return characterClassOnly; } }

    public Color CharacterColor { get { return characterColor; } }
    public Sprite Portrait { get { return portrait; } }

    public string Description { get { return description; } }

    public bool RandomStats { get { return randomStats; } }

    public WeaponMod StartWeaponMod
    {
        get
        {
            if (!randomStats)
                return startWeaponMod;
            else if (RNG_startWeaponMod.Length > 0)
                return RNG_startWeaponMod[Random.Range(0, RNG_startWeaponMod.Length)];
            else
                return null;
        }
    }
    public int Ammunition
    {
        get
        {
            if (!randomStats)
                return ammunition;
            else
                return (int)Random.Range(RNG_ammunition.x, RNG_ammunition.y);
        }
    }
    public int MaxStorageAmmo
    {
        get
        {
            if (!randomStats)
                return maxStorageAmmo;
            else
                return (int)Random.Range(RNG_maxStorageAmmo.x, RNG_maxStorageAmmo.y);
        }
    }

    public MeleeWeapon StartMeleeWeapon
    {
        get
        {
            if (!randomStats)
                return startMeleeWeapon;
            else if (RNG_startMeleeWeapon.Length > 0)
                return RNG_startMeleeWeapon[Random.Range(0, RNG_startMeleeWeapon.Length)];
            else
                return null;
        }
    }

    public Pickup_Enhancer[] SuitMods
    {
        get
        {
            if (!randomStats)
                return suitMods;
            else if (RNG_suitMods.Length > 0)
            {
                List<Pickup_Enhancer> tempSuitMods = new List<Pickup_Enhancer>();
                int nmbrOfSuitMods = (int)Random.Range(RNG_amountOfSuitMods.x, RNG_amountOfSuitMods.y);
                nmbrOfSuitMods = Mathf.Max(nmbrOfSuitMods, RNG_suitMods.Length);
                for (int i = 0; i < nmbrOfSuitMods; i++)
                {
                    int index = Random.Range(0, RNG_suitMods.Length);
                    if (!tempSuitMods.Contains(RNG_suitMods[index]))
                    {
                        tempSuitMods.Add(RNG_suitMods[index]);
                    }
                }
                return tempSuitMods.ToArray();
            }
            else
                return null;
        }
    }

    public float Health
    {
        get
        {
            if (!randomStats)
                return health;
            else
                return Random.Range(RNG_health.x, RNG_health.y);
        }
    }
    public float MaxHealth
    {
        get
        {
            if (!randomStats)
                return maxHealth;
            else
                return Random.Range(RNG_maxHealth.x, RNG_maxHealth.y);
        }
    }
    public float Regeneration
    {
        get
        {
            if (!randomStats)
                return regeneration;
            else
                return Random.Range(RNG_regeneration.x, RNG_regeneration.y);
        }
    }
    public float Resistance
    {
        get
        {
            if (!randomStats)
                return resistance;
            else
                return Random.Range(RNG_resistance.x, RNG_resistance.y);
        }
    }
    public float Speed
    {
        get
        {
            if (!randomStats)
                return speed;
            else
                return Random.Range(RNG_speed.x, RNG_speed.y);
        }
    }

    private void OnValidate()
    {
        nonRandomStats = !randomStats;
    }
}

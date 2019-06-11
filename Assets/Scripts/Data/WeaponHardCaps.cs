using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponstatsHardCaps", menuName = "Weapon/WeaponstatsHardCaps")]
public class WeaponHardCaps : ScriptableObject
{
    [SerializeField] WeaponStats minCaps;
    [SerializeField] WeaponStats maxCaps;

    public WeaponStats MinCaps { get { return minCaps; } }
    public WeaponStats MaxCaps { get { return maxCaps; } }
}

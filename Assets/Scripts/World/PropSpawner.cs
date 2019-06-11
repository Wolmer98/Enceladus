using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum SizeType { All, Pickup, Size1x_1y_1z, Size2x_2y_2z, Size3x_3y_3z, Size1x_2y_2z, Size2x_1y_2z, Size2x_2y_1z, Size1x_1y_2z, Size1x_2y_1z, Size2x_1y_1z }
public enum PropType { All, ModTerminal, Pickup, Box, Bookshelf, Sofa }

public class PropSpawner : MonoBehaviour
{
    [Header("Destroy On Start Settings")]
    [SerializeField] bool destroyOnStart;
    [ShowIf("destroyOnStart")] [Range(0, 1)] [SerializeField] float destroyChance;

    [Space]
    [Tooltip("If this filed is assigned, the spawner will only spawn this specific object and nothing else.")]
    [SerializeField] Prop SpecificPropToSpawn;

    [Header("Prop Settings")]
    [SerializeField] SizeType sizeType;
    [SerializeField] PropType propType;

    Transform propParent;
    GameObject spawnedProp;

    HashSet<Prop> propSet = new HashSet<Prop>();
    HashSet<Prop> commonSet = new HashSet<Prop>();
    HashSet<Prop> rareSet = new HashSet<Prop>();
    HashSet<Prop> epicSet = new HashSet<Prop>();
    HashSet<Prop> legendarySet = new HashSet<Prop>();


    private void Awake()
    {
        if (destroyOnStart)
        {
            float rng = Random.Range(0f, 1f);
            if (rng <= destroyChance)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Builds this spawnpoints proplist.
    /// </summary>
    public void BuildPropList(List<Prop> allProps)
    {
        foreach (Prop p in allProps)
        {
            if (p != null)
            {
                if ((p.sizeType == sizeType || sizeType == SizeType.All) && (p.propType == propType || propType == PropType.All))
                {
                    switch (p.propRarity)
                    {
                        case PropRarity.Common:
                            commonSet.Add(p);
                            break;
                        case PropRarity.Rare:
                            rareSet.Add(p);
                            break;
                        case PropRarity.Epic:
                            epicSet.Add(p);
                            break;
                        case PropRarity.Legendary:
                            legendarySet.Add(p);
                            break;
                    }
                    propSet.Add(p);
                }
            }
        }
    }

    //public void RemovePropFromPropList(object prop)
    //{
    //    for (int i = 0; i < propList.Count; i++)
    //    {
    //        if (propList[i] != null && propList[i].GetComponent(prop.GetType()) != null &&
    //            propList[i].GetComponent(prop.GetType()).GetType() == prop.GetType())
    //        {
    //            //Debug.Log("REMOVED AN ITEM FROM PROPLIST");
    //            propList.RemoveAt(i);

    //            if (SpecificPropToSpawn != null && SpecificPropToSpawn.GetComponent(prop.GetType()).GetType() == prop.GetType())
    //            {
    //                //Debug.Log("REMOVED A SPECIFIC ITEM FROM SPECIFIC ITEM TO SPAWN");
    //                SpecificPropToSpawn = null;
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// Spawns either one of the props that matches the spawners prop-settings. Or one specific prop which has been assigned.
    /// </summary>
    public void SpawnProp(bool firstSpawn = true)
    {
        if (firstSpawn)
        {
            propParent = transform.GetChild(0);
            Destroy(propParent.GetChild(0).gameObject);
        }
        else
        {
            Destroy(spawnedProp.gameObject);
        }


        if (SpecificPropToSpawn != null)
        {
            spawnedProp = Instantiate(SpecificPropToSpawn.gameObject, propParent);
            return;
        }

        if (propSet.Count > 0)
        {
            int rng = Random.Range(0, propSet.Count);
            Prop prop = GetPropBasedOnRarity(propSet);

            if (prop != null)
            {
                spawnedProp = Instantiate(prop.gameObject, propParent);
                propParent.parent.name = propParent.name + "_" + spawnedProp.name;
            }
        }
    }

    private Prop GetPropBasedOnRarity(HashSet<Prop> props)
    {
        PropRarityChances rarityChances = FindObjectOfType<WorldGenerator>().propRarityChances;
        float chance = Random.Range(0f, 1f);

        HashSet<Prop> pickableProps = new HashSet<Prop>();

        if (chance <= rarityChances.legendaryChance) // Legendary event chance.
        {
            pickableProps.UnionWith(legendarySet);
        }
        if (chance <= rarityChances.epicChance) // Epic event chance.
        {
            pickableProps.UnionWith(epicSet);
        }
        if (chance <= rarityChances.rareChance) // Rare event chance.
        {
            pickableProps.UnionWith(rareSet);
        }
        if (chance <= rarityChances.commonChance) // Common event chance.
        {
            pickableProps.UnionWith(commonSet);
        }

        pickableProps.IntersectWith(props);
        List<Prop> propList = new List<Prop>(pickableProps);

        if (propList.Count == 0)
        {
            Debug.LogError("No props with weight could be found. Make sure you've set the 'Prop Rarity Chances' in the World Generator.", this);
            return null;
        }

        return propList[Random.Range(0, propList.Count)];
    }

    private void OnValidate()
    {
        if (SpecificPropToSpawn != null)
        {
            sizeType = SpecificPropToSpawn.sizeType;
            propType = SpecificPropToSpawn.propType;
        }

        propParent = transform.GetChild(0);
        propParent.GetChild(0).localScale = GetPropSize();
        Vector3 tempPos = propParent.GetChild(0).position;
        propParent.GetChild(0).position = new Vector3(tempPos.x, tempPos.y, tempPos.z);
    }

    private Vector3 GetPropSize()
    {
        switch (sizeType)
        {
            case SizeType.Pickup:
                return new Vector3(0.3f, 0.3f, 0.3f);
            case SizeType.Size1x_1y_1z:
                return new Vector3(1, 1, 1);
            case SizeType.Size1x_2y_1z:
                return new Vector3(1, 2, 1);
            case SizeType.Size3x_3y_3z:
                return new Vector3(3, 3, 3);
            case SizeType.Size1x_1y_2z:
                return new Vector3(1, 1, 2);
            case SizeType.Size2x_1y_2z:
                return new Vector3(2, 1, 2);
            case SizeType.Size2x_2y_2z:
                return new Vector3(2, 2, 2);
            case SizeType.Size2x_2y_1z:
                return new Vector3(2, 2, 1);
            case SizeType.Size2x_1y_1z:
                return new Vector3(2, 1, 1);
            case SizeType.Size1x_2y_2z:
                return new Vector3(1, 2, 2);
            default:
                return new Vector3(1, 1, 1);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum PropRarity {Common, Rare, Epic, Legendary};

public class Prop : MonoBehaviour
{
    public SizeType sizeType;
    public PropType propType;
    public PropRarity propRarity;

    [SerializeField] bool destroyOnStart = false;
    [ShowIf("destroyOnStart")] [SerializeField] [Range(0,1)] float destroyChance = 0;

    private void Start()
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
}

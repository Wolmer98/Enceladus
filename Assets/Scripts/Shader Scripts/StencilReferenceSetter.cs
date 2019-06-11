using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StencilReferenceSetter : MonoBehaviour
{
    void Start()
    {
        FindObjectOfType<WorldGenerator>().OnWorldStart.AddListener(delegate { Init(); });
    }

    private void Init()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        Transform room = transform;
        while (!room.name.Contains("Room") && room.parent != null)
        {
            room = room.parent;
        }
        int referenceNumber = (room.GetSiblingIndex() + 1) % 4999; //Ref number goes between 1-255

        foreach (Renderer r in renderers)
        {
            Material[] materials = r.sharedMaterials;

            foreach (Material m in materials)
            {
                //m.renderQueue += referenceNumber;
            }
        }
    }
}

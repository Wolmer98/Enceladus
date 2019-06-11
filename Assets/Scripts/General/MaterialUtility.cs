using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialUtility : MonoBehaviour
{
    /// <summary>
    /// Changes the material to the given material.
    /// </summary>
    public void ChangeMaterial(Material material)
    {
        GetComponent<Renderer>().material = material;
    }
}

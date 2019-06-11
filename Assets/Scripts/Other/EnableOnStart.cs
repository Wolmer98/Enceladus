using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnableOnStart : MonoBehaviour
{

    public GameObject[] gameObjects;
    public Image image;

    private void Awake()
    {
        FindObjectOfType<WorldGenerator>().OnWorldStart.AddListener(InitAfterWorldGen);
    }

    private void Start()
    {
        if (image != null)
        {
            image.enabled = true;
        }
    }

    private void InitAfterWorldGen()
    {
        if (gameObjects != null)
        {
            foreach (GameObject g in gameObjects)
                g.SetActive(true);
        }
    }
}

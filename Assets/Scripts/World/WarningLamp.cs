using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLamp : MonoBehaviour
{


    bool flickering = true;

    public int speed;

    public int max;

    Light light; 


    void Start()
    {
        light = GetComponent<Light>();
    }


    void Update()
    {
        light.range = ((Mathf.Sin(Time.time * speed) + 1.0f) / 2.0f) * max;
    }
}
